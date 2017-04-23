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

namespace GoToDnSpy
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GoToDnSpy
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("4843d15e-dca8-4935-8ba3-66c4d25fb295");

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
        /// Initializes a new instance of the <see cref="GoToDnSpy"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private GoToDnSpy(Package package)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            _package = package;

            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }

            _statusBar = (IVsStatusbar) ServiceProvider.GetService(typeof(SVsStatusbar));

            if (_statusBar == null)
                throw new ArgumentNullException(nameof(_statusBar));

            _componentModel = (IComponentModel) ServiceProvider.GetService(typeof(SComponentModel));

            if (_componentModel == null)
                throw new ArgumentNullException(nameof(_componentModel));

            _editorAdaptersFactory = _componentModel.GetService<IVsEditorAdaptersFactoryService>();

            if (_editorAdaptersFactory == null)
                throw new ArgumentNullException(nameof(_editorAdaptersFactory));

        }





        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new GoToDnSpy(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                var dnSpyPath = ReadDnSpyPath();
                if(string.IsNullOrWhiteSpace(dnSpyPath))
                {
                    ShowMsgBox("Set dnSpy path in options first!", "Error");
                    return;
                }

                if(!File.Exists(dnSpyPath))
                {
                    ShowMsgBox($"File '{dnSpyPath}' not exists!", "Error");
                    return;
                }

                var textView = GetTextView();
                SnapshotPoint caretPosition = textView.Caret.Position.BufferPosition;
                Microsoft.CodeAnalysis.Document document = caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
                if (document == null)
                {
                    _statusBar.SetText("Execute the function while a document window is active.");
                    return;
                }

                SyntaxNode rootSyntaxNode = document.GetSyntaxRootAsync().Result;
                SyntaxToken st = rootSyntaxNode.FindToken(caretPosition);
                SemanticModel semanticModel = document.GetSemanticModelAsync().Result;
                SymbolInfo si = semanticModel.GetSymbolInfo(st.Parent);
                ISymbol symbol = si.Symbol ?? (si.GetType().GetProperty("CandidateSymbols").GetValue(si) as IEnumerable<ISymbol>)?.FirstOrDefault();

                PreprocessSymbol(ref symbol);

                INamedTypeSymbol typeSymbol = null;
                string memberName = null;

                MemberType memberType = 0;

                // todo: view SLaks.Ref12.Services.RoslynSymbolResolver
                if ((symbol == null) || ((!TryHandleAsType(symbol, out typeSymbol)) && (!TryHandleAsMember(symbol, out typeSymbol, out memberName, out memberType))))
                {
                    _statusBar.SetText($"{st.Text} is not a valid identifier.");
                    return;
                }

                string typeNamespace = GetFullNamespace(typeSymbol);
                string typeName =   typeNamespace + "." + typeSymbol.MetadataName;
                string asmDef = GetAssemblyDefinition(typeSymbol.ContainingAssembly);
                string asmPath = GetAssemblyPath(semanticModel, asmDef);

                if (asmPath == null || !File.Exists(asmPath))
                {
                    _statusBar.SetText($"{typeName} is not referenced assembly type. Assembly '{asmDef}' not found;");
                    return;
                }



                System.Diagnostics.Process.Start(@"Q:\coding\dnSpy\dnSpy.exe", BuildDnSpyArguments(asmPath, typeName, memberName, memberType));
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GoToDnSpy] Exception: {ex}");
                _statusBar.SetText(ex.Message.ToString());
                ShowMsgBox(ex.ToString(), "Error");
                
            }
        }

        string ReadDnSpyPath()
        {
            // DTE dte = (DTE) ServiceProvider.GetService(typeof(DTE));
            // EnvDTE.Properties props = dte.get_Properties("GoTo dnSpy", "General");
            //return props.Item("DnSpyPath")?.Value?.ToString();
            return ((SettingsDialog) _package.GetDialogPage(typeof(SettingsDialog)))?.DnSpyPath;
        }

        static string BuildDnSpyArguments(string asmPath, string typeName, string memberName, MemberType memberType)
        {
            string result = $"\"{asmPath}\" --dont-load-files --select {(memberName == null ? 'T' : memberType.ToString()[0])}:{typeName}";
            if (memberName != null)
                result = $"{result}.{memberName}";
            return result;
        }


        static string GetAssemblyPath(SemanticModel semanticModel, string assemblyDef)
        {
            IEnumerator<AssemblyIdentity> refAsmNames = semanticModel.Compilation.ReferencedAssemblyNames.GetEnumerator();
            IEnumerator<MetadataReference> refs = semanticModel.Compilation.References.GetEnumerator();

            while (refAsmNames.MoveNext())
            {
                refs.MoveNext();
                if (refAsmNames.Current.ToString() == assemblyDef)
                    return refs.Current.Display;
            }
            return null;
        }

        #region process symbol
        static void PreprocessSymbol(ref ISymbol symbol)
        {
            if (TryPreprocessLocal(ref symbol))
                return;
        }

        static bool TryPreprocessLocal(ref ISymbol symbol)
        {
            ILocalSymbol loc = symbol as ILocalSymbol;
            bool result = loc != null;
            if (result)
                symbol = loc.Type;
            return result;
        }

        static bool TryHandleAsType(ISymbol symbol, out INamedTypeSymbol type)
        {
            type = symbol as INamedTypeSymbol;
            return type != null;
        }

        static bool TryHandleAsMember(ISymbol symbol, out INamedTypeSymbol type, out string memberName, out MemberType memberType)
        {
            type = null;
            memberName = null;
            memberType = 0;

            IFieldSymbol fieldSymbol = symbol as IFieldSymbol;
            if (fieldSymbol != null)
            {
                type = fieldSymbol.ContainingType;
                memberName = fieldSymbol.Name;
                memberType = MemberType.Field;
            }
            else
            {
                IPropertySymbol propSymbol = symbol as IPropertySymbol;
                if (propSymbol != null)
                {
                    type = propSymbol.ContainingType;
                    memberName = propSymbol.Name;
                    memberType = MemberType.Property;
                }
                else
                {
                    IMethodSymbol methSymbol = symbol as IMethodSymbol;
                    if (methSymbol != null)
                    {
                        type = methSymbol.ContainingType;
                        memberName = methSymbol.Name;
                        memberType = MemberType.Method;
                    }
                    else
                    {
                        IEventSymbol eventSymbol = symbol as IEventSymbol;
                        if (eventSymbol != null)
                        {
                            type = eventSymbol.ContainingType;
                            memberName = eventSymbol.Name;
                            memberType = MemberType.Event;
                        }
                    }
                }
            }
            return type != null;
        }

        enum MemberType { Field, Property, Method, Event }
        #endregion process symbol

        private static string GetAssemblyDefinition(IAssemblySymbol assemblySymbol)
        {
            return assemblySymbol.Identity.ToString();
        }

        private static string GetRealAssemblyPath(string asmPath)
        {
            try
            {
                return Assembly.LoadFile(asmPath)?.Location ?? asmPath;
            }
            catch
            { }
            return asmPath;
        }

        private static string GetFullNamespace(INamedTypeSymbol typeSymbol)
        {
            INamespaceSymbol nsSym = typeSymbol.ContainingNamespace;
            var sb = new StringBuilder();
            while ((nsSym != null) && (!nsSym.IsGlobalNamespace))
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
            var textManager = (IVsTextManager) ServiceProvider.GetService(typeof(SVsTextManager));
            IVsTextView textView;
            textManager.GetActiveView(1, null, out textView);
            return _editorAdaptersFactory.GetWpfTextView(textView);
        }

        private void ShowMsgBox(string message, string title)
        {
            VsShellUtilities.ShowMessageBox(
                            this.ServiceProvider,
                            message,
                            title,
                            OLEMSGICON.OLEMSGICON_INFO,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
