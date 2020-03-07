using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using EnvDTE;
using System.Windows.Forms;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Threading;
using System.Security.Cryptography;

namespace GoToDnSpy
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed partial class GoToDnSpy
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// The status bar
        /// </summary>
        private readonly IVsStatusbar _statusBar;

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GoToDnSpy Instance { get; private set; }

        /// <summary>
        /// Netcore/netstandard libs can returns us reference assembly netstandard.dll, we don't know how to map it. wi'll try to find right assembly
        /// </summary>
        public readonly static NamespaceToAssemblyMapper NetstandardMapper = new NamespaceToAssemblyMapper();

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// The editor adapters factory
        /// </summary>
        private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactory;

        /// <summary>
        /// COM
        /// </summary>
        private readonly IComponentModel _componentModel;

        /// <summary>
        /// EnvDTE service
        /// </summary>
        private readonly DTE _dte;

        private readonly static char[] _pathTrimChars = new[] { '\r', '\n', ' ', '\'', '\"' };

        /// <summary>
        /// Initializes a new instance of the <see cref="GoToDnSpy"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private GoToDnSpy(Package package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                _package = package ?? throw new ArgumentNullException(nameof(package));

                if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
                {
                    // Add tools menu command
                    var menuCommandId = new CommandID(PackageGuids.guidGoToDnSpyPackageCmdSetMenu, PackageIds.GoToDnSpyId);
                    var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandId);
                    commandService.AddCommand(menuItem);
                    // Add editor context menu command (right click)
                    var ctxMenuCommand = new CommandID(PackageGuids.guidGoToDnSpyPackageCmdSetContextMenu, PackageIds.GoToDnSpyContextMenuId);
                    var ctxMenuItem = new MenuCommand(this.MenuItemCallback, ctxMenuCommand);
                    commandService.AddCommand(ctxMenuItem);
                }

                _statusBar = (IVsStatusbar)ServiceProvider.GetService(typeof(SVsStatusbar));
                Assumes.Present(_statusBar);

                _componentModel = (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));
                Assumes.Present(_componentModel);

                _editorAdaptersFactory = _componentModel.GetService<IVsEditorAdaptersFactoryService>();
                Assumes.Present(_editorAdaptersFactory);

                _dte = (DTE)ServiceProvider.GetService(typeof(DTE));
                Assumes.Present(_dte);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Some error in GoToDnSpy extensiton.\n Please take screenshot and create issue on github with this error\n{ex}", "[GoToDnSpy] Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package) => Instance = new GoToDnSpy(package);

        public static void Output(string msg)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Get the output window
            var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

            // Ensure that the desired pane is visible
            var paneGuid = VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            outputWindow.CreatePane(paneGuid, "General", 1, 0);
            outputWindow.GetPane(paneGuid, out IVsOutputWindowPane pane);

            // Output the message
            pane.OutputString(msg);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void MenuItemCallback(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _statusBar.SetText("");
                var dnSpyPath = ReadDnSpyPath()?.Trim(_pathTrimChars);
                if (string.IsNullOrWhiteSpace(dnSpyPath))
                {
                    MessageBox.Show("You must specify dnSpy path in options", "[GoToDnSpy] Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!File.Exists(dnSpyPath))
                {
                    MessageBox.Show($"File '{dnSpyPath}' doesn't exist!", "[GoToDnSpy] Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var textView = GetTextView();
                if (textView == null)
                {
                    _statusBar.SetText("Can't get a text view, please open the file and execute the command while a document window is active.");
                    return;
                }
                SnapshotPoint caretPosition = textView.Caret.Position.BufferPosition;
                Microsoft.CodeAnalysis.Document document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
                if (document == null)
                {
                    _statusBar.SetText("You should execute the command while a document window is active.");
                    return;
                }

                SyntaxNode rootSyntaxNode = await document.GetSyntaxRootAsync().ConfigureAwait(true);
                SyntaxToken st = rootSyntaxNode.FindToken(caretPosition);
                SemanticModel semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(true);

                ISymbol symbol = null;
                var parentKind = st.Parent.Kind();
                if (st.Kind() == SyntaxKind.IdentifierToken && (
                       parentKind == SyntaxKind.PropertyDeclaration
                    || parentKind == SyntaxKind.FieldDeclaration
                    || parentKind == SyntaxKind.MethodDeclaration
                    || parentKind == SyntaxKind.NamespaceDeclaration
                    || parentKind == SyntaxKind.DestructorDeclaration
                    || parentKind == SyntaxKind.ConstructorDeclaration
                    || parentKind == SyntaxKind.OperatorDeclaration
                    || parentKind == SyntaxKind.ConversionOperatorDeclaration
                    || parentKind == SyntaxKind.EnumDeclaration
                    || parentKind == SyntaxKind.EnumMemberDeclaration
                    || parentKind == SyntaxKind.ClassDeclaration
                    || parentKind == SyntaxKind.EventDeclaration
                    || parentKind == SyntaxKind.EventFieldDeclaration
                    || parentKind == SyntaxKind.InterfaceDeclaration
                    || parentKind == SyntaxKind.StructDeclaration
                    || parentKind == SyntaxKind.DelegateDeclaration
                    || parentKind == SyntaxKind.IndexerDeclaration
                    || parentKind == SyntaxKind.VariableDeclarator
                    ))
                {
                    symbol = semanticModel.LookupSymbols(caretPosition.Position, name: st.Text).FirstOrDefault();
                }
                else
                {
                    SymbolInfo si = semanticModel.GetSymbolInfo(st.Parent);
                    symbol = si.Symbol ?? si.CandidateSymbols.FirstOrDefault();
                }

                TryPreprocessLocal(ref symbol);

                string memberName = null;

                MemberType memberType = 0;

                // todo: view SLaks.Ref12.Services.RoslynSymbolResolver
                if (symbol == null || (
                        (!TryHandleAsType(symbol, out INamedTypeSymbol typeSymbol))
                     && (!TryHandleAsMember(symbol, out typeSymbol, out memberName, out memberType))
                ))
                {
                    var msg = $"{st.Text} is not a valid identifier. token: {st}, Kind: {st.Kind()}";
                    _statusBar.SetText(msg);
                    Debug.WriteLine(msg);
                    return;
                }

                string typeNamespace = GetFullNamespace(typeSymbol);
                string typeName = typeNamespace + "." + typeSymbol.MetadataName;
                string asmDef = GetAssemblyDefinition(typeSymbol.ContainingAssembly);
                string asmPath = GetAssemblyPath(semanticModel, asmDef);

                if (string.IsNullOrWhiteSpace(asmPath))
                {
                    _statusBar.SetText($"Assembly '{asmDef}' with type {typeName} not found;");
                    return;
                }
                else if (!File.Exists(asmPath))
                {
                    MessageBox.Show($"Try build project first;\nAssembly '{asmDef}' with type {typeName} not found, path:\n{asmPath}", "[GoToDnSpy] Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // for netstandard path will be c:\▮▮▮▮\dotnet\sdk\packs\NETStandard.Library.Ref\2.1.0\ref\netstandard2.1\
                // also can be with netstandard.library (via nupkg)
                if (asmPath.IndexOf("NETStandard.Library.Ref", StringComparison.Ordinal) > 0
                    || asmPath.IndexOf("netstandard.library", StringComparison.Ordinal) > 0
                    || asmPath.IndexOf("microsoft.netcore.app.ref", StringComparison.Ordinal) > 0)
                {

                    const string baseUrl = "https://source.dot.net";
                    var docId = GetDocumentationCommentId(memberName is null ? typeSymbol : symbol);
                    var assemblyName = NetstandardMapper.Get(typeNamespace, typeName);
                    if (assemblyName != null)
                    {
                        var url = baseUrl + "/" + assemblyName + "/a.html#" + GetMD5Hash(docId, 16);
                        System.Diagnostics.Process.Start(new ProcessStartInfo() {
                            UseShellExecute = true,
                            FileName = url,
                        });
                        return;
                    }
                }
                System.Diagnostics.Process.Start(dnSpyPath, BuildDnSpyArguments(asmPath, typeName, memberName, memberType));

            }
            catch (Exception ex)
            {
                _statusBar.SetText(ex.Message);
                MessageBox.Show($"Some error in GoToDnSpy extensiton.\n Please take a screenshot and create issue on github with this error\n{ex}", "[GoToDnSpy] Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetDocumentationCommentId(ISymbol symbol)
        {
            if (!symbol.IsDefinition)
            {
                symbol = symbol.OriginalDefinition;
            }

            string result = symbol.GetDocumentationCommentId();

            if (result == null)     // goto labels have no doc comment ID
                return null;

            result = result.Replace("#ctor", "ctor");

            return result;
        }


        private static string GetMD5Hash(string input, int digits)
        {
            using var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(bytes);
            return ByteArrayToHexString(hashBytes, digits);
        }

        private static string ByteArrayToHexString(byte[] bytes, int digits = 0)
        {
            if (digits == 0)
                digits = bytes.Length * 2;

            char[] c = new char[digits];
            byte b;
            for (int i = 0; i < digits / 2; i++)
            {
                b = (byte)(bytes[i] >> 4);
                c[i * 2] = (char)(b > 9 ? b + 87 : b + 0x30);
                b = (byte)(bytes[i] & 0xF);
                c[(i * 2) + 1] = (char)(b > 9 ? b + 87 : b + 0x30);
            }
            return new string(c);
        }




        private string GetCurrentFileOutputAssembly()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var project = _dte.ActiveDocument?.ProjectItem?.ContainingProject;

            // if it's fake project, this object doesn't have ConfigurationManager, we try find another object
            if (project != null && (
                        string.CompareOrdinal(project.UniqueName, EnvDTE.Constants.vsMiscFilesProjectUniqueName) == 0
                     || string.CompareOrdinal(project.UniqueName, EnvDTE.Constants.vsSolutionItemsProjectUniqueName) == 0
                ))
            {
                project = _dte.Solution.FindProjectItem(_dte.ActiveDocument.FullName)?.ContainingProject;
            }

            if (project == null)
                return null;

            return GetTargetOutputPath(project);
        }

        private string GetTargetOutputPath(EnvDTE.Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string outputPath = project.GetPropertyOrDefault("OutputPath");

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                _statusBar.SetText($"Can't find output path of project {project.Name}!");
                return null;
            }

            string directory;
            string outputFilename = project.FileName;
            if (string.IsNullOrWhiteSpace(outputFilename))
            {
                outputFilename = project.FullName;
            }

            // check outputPath type (shares and C:\ is absolute path and we can just return)
            if (outputPath.StartsWith("\\\\", StringComparison.Ordinal)
                || (outputPath.Length >= 2 && outputPath[1] == Path.VolumeSeparatorChar))
            {
                directory = outputPath;
            }
            else
            {
                directory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(outputFilename), outputPath));
            }

            return Path.Combine(directory, project.GetOutputFilename() ?? Path.ChangeExtension(outputFilename, ".dll"));
        }

        private string ReadDnSpyPath() => ((SettingsDialog)_package.GetDialogPage(typeof(SettingsDialog)))?.DnSpyPath;

        private static string BuildDnSpyArguments(string asmPath, string typeName, string memberName, MemberType memberType)
        {
            string result = $"\"{asmPath}\" --dont-load-files --select {(memberName == null ? 'T' : memberType.ToString()[0])}:{typeName}";
            if (memberName != null)
                result = $"{result}.{memberName}";
            return result;
        }

        private string GetAssemblyPath(SemanticModel semanticModel, string assemblyDef)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IEnumerator<AssemblyIdentity> refAsmNames = semanticModel.Compilation.ReferencedAssemblyNames.GetEnumerator();
            IEnumerator<MetadataReference> refs = semanticModel.Compilation.References.GetEnumerator();

            // try find in referenced assemblies first
            while (refAsmNames.MoveNext())
            {
                refs.MoveNext();
                if (!string.Equals(refAsmNames.Current.ToString(), assemblyDef, StringComparison.OrdinalIgnoreCase))
                    continue;

                var displayName = refs.Current.Display;
                EnvDTE.Project project = null;

                // try found project
                foreach (EnvDTE.Project proj in _dte.Solution.Projects)
                {
                    if (!string.Equals(proj.Name, displayName, StringComparison.OrdinalIgnoreCase))
                        continue;
                    project = proj;
                    break;
                }
                // not found, reference is a path
                if (project == null)
                {
                    return TryCorrectReferenceAssemblyPath(displayName, assemblyDef);
                }

                // project reference
                return GetTargetOutputPath(project) ?? displayName;
            }

            // we in same assembly that symbol, try find output path
            if (semanticModel.Compilation.Assembly.Identity.ToString() == assemblyDef)
            {
                var outputPath = GetCurrentFileOutputAssembly();

                if (!string.IsNullOrWhiteSpace(outputPath))
                    return outputPath;
            }

            return null;
        }

        /// <summary>
        /// Find type of local symbol
        /// </summary>
        private static bool TryPreprocessLocal(ref ISymbol symbol)
        {
            if (symbol is ILocalSymbol loc)
            {
                symbol = loc.Type;
                return true;
            }
            return false;
        }

        private static bool TryHandleAsType(ISymbol symbol, out INamedTypeSymbol type)
        {
            type = symbol as INamedTypeSymbol;
            return type != null;
        }

        private static bool TryHandleAsMember(ISymbol symbol, out INamedTypeSymbol type, out string memberName, out MemberType memberType)
        {
            if (symbol is IFieldSymbol fieldSymbol)
            {
                type = fieldSymbol.ContainingType;
                memberName = fieldSymbol.Name;
                memberType = MemberType.Field;
            }
            else if (symbol is IPropertySymbol propSymbol)
            {
                type = propSymbol.ContainingType;
                memberName = propSymbol.Name;
                memberType = MemberType.Property;
            }
            else if (symbol is IMethodSymbol methodSymbol)
            {
                type = methodSymbol.ContainingType;
                memberName = methodSymbol.Name;
                memberType = MemberType.Method;
            }
            else if (symbol is IEventSymbol eventSymbol)
            {
                type = eventSymbol.ContainingType;
                memberName = eventSymbol.Name;
                memberType = MemberType.Event;
            }
            else
            {
                type = null;
                memberName = null;
                memberType = 0;
            }

            return type != null;
        }

        private static string GetAssemblyDefinition(IAssemblySymbol assemblySymbol)
            => assemblySymbol.Identity.ToString();

        private static string GetFullNamespace(INamedTypeSymbol typeSymbol)
        {
            INamespaceSymbol nsSym = typeSymbol.ContainingNamespace;
            var sb = new StringBuilder();
            while (nsSym?.IsGlobalNamespace == false)
            {
                if (sb.Length == 0)
                    sb.Append(nsSym.Name);
                else
                    sb.Insert(0, '.').Insert(0, nsSym.Name);
                nsSym = nsSym.ContainingNamespace;
            }

            return sb.ToString();
        }

        private IWpfTextView GetTextView()
        {
            var textManager = (IVsTextManager)ServiceProvider.GetService(typeof(VsTextManagerClass));
            Assumes.Present(textManager);
            if (textManager.GetActiveView(fMustHaveFocus: 0, null, out IVsTextView textView) != 0)
                return null;
            return _editorAdaptersFactory.GetWpfTextView(textView);
        }

        /// <summary>
        /// Assemblies in "C:\Program Files\Reference Assemblies\" doesn't have implementation.
        /// Try to get the assembly from GAC
        /// See https://github.com/vchirikov/GoToDnSpy/issues/2
        /// </summary>
        private static string TryCorrectReferenceAssemblyPath(string path, string assemblyDef)
        {
            if (!GacHelper.IsReferenceAssembly(path))
                return path;

            var netfxGacPath = GacHelper.FindAssemblyInGac(assemblyDef);

            return netfxGacPath ?? path;
        }
    }
}
