using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoToDnSpy
{
    public class NamespaceToAssemblyMapper
    {
        private readonly static char[] _separator = new char[] { '.' };
        private readonly static string[] _coreLibStartWithNamespaces = new string[] {
            "System.Reflection",
            "System.Runtime",
            "System.Diagnostics",
            "System.Reflection",
            "System.Text",
            "System.Buffers",
            "System.Threading",
            "System.Net",
            "System.Numerics",
        };

        private readonly static string[] _coreLibStandAloneNamespaces = new string[] {
            "System",
            "System.IO",
            "System.Globalization",
            "System.Collections.Generic",
        };

        private readonly static string[] _coreLibTypes = new string[] {
            "System.Collections.Concurrent.ConcurrentQueue",
            "System.Collections.Comparer",
        };


        internal string Get(string typeNamespace, string typeName)
        {
            if (_coreLibStartWithNamespaces.Any(x => x.StartsWith(typeNamespace))
                || _coreLibStandAloneNamespaces.Contains(typeNamespace)
                || _coreLibTypes.Any(x => x.StartsWith(typeName)))
            {
                return "System.Private.CoreLib";
            }

            var result = CheckNamespace(typeNamespace);
            if (result != null)
                return result;

            var parts = typeNamespace.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder(256);
            for (int i = parts.Length; i > 0; i--)
            {
                sb.Clear();
                for (int j = 0; j < i; j++)
                    sb.Append(parts[j]).Append('.');
                sb.Length--;
                result = CheckNamespace(sb.ToString());
                if (result != null)
                    return result;
            }
            // good luck when
            return typeNamespace.StartsWith("System", StringComparison.Ordinal) ? "System.Private.CoreLib" : null;

            static string CheckNamespace(string ns)
            {
                foreach (var asm in _availableAssemblies)
                {
                    if (asm.StartsWith(ns, StringComparison.Ordinal))
                        return asm;
                }
                return null;
            }
        }

        #region Data
        private static readonly string[] _availableAssemblies = new string[] {
"ALCProxy.Communication",
"ALCProxy.Proxy",
"blazor-devserver",
"BoardLed",
"Buzzer",
"CreditCardProcessing",
"DCMotor",
"DevDivPackage",
"Dhtxx",
"Display",
"ExplorerHat",
"GetDocument.Insider",
"GrovePiDevice",
"GuidAssembly",
"Ignitor",
"Internal.AspNetCore.Analyzers",
"Iot.Device.Bindings",
"Localization",
"Media",
"Microsoft.Activities.Build",
"Microsoft.AspNetCore",
"Microsoft.AspNetCore.Analyzers",
"Microsoft.AspNetCore.Antiforgery",
"Microsoft.AspNetCore.ApiAuthorization.IdentityServer",
"Microsoft.AspNetCore.App.Runtime",
"Microsoft.AspNetCore.Authentication",
"Microsoft.AspNetCore.Authentication.Abstractions",
"Microsoft.AspNetCore.Authentication.AzureAD.UI",
"Microsoft.AspNetCore.Authentication.AzureADB2C.UI",
"Microsoft.AspNetCore.Authentication.Certificate",
"Microsoft.AspNetCore.Authentication.Cookies",
"Microsoft.AspNetCore.Authentication.Core",
"Microsoft.AspNetCore.Authentication.Facebook",
"Microsoft.AspNetCore.Authentication.Google",
"Microsoft.AspNetCore.Authentication.JwtBearer",
"Microsoft.AspNetCore.Authentication.MicrosoftAccount",
"Microsoft.AspNetCore.Authentication.Negotiate",
"Microsoft.AspNetCore.Authentication.OAuth",
"Microsoft.AspNetCore.Authentication.OpenIdConnect",
"Microsoft.AspNetCore.Authentication.Twitter",
"Microsoft.AspNetCore.Authentication.WsFederation",
"Microsoft.AspNetCore.Authorization",
"Microsoft.AspNetCore.Authorization.Policy",
"Microsoft.AspNetCore.AzureAppServices.HostingStartup",
"Microsoft.AspNetCore.AzureAppServices.SiteExtension",
"Microsoft.AspNetCore.AzureAppServicesIntegration",
"Microsoft.AspNetCore.BenchmarkRunner.Sources",
"Microsoft.AspNetCore.Blazor",
"Microsoft.AspNetCore.Blazor.DataAnnotations.Validation",
"Microsoft.AspNetCore.Blazor.HttpClient",
"Microsoft.AspNetCore.Blazor.Server",
"Microsoft.AspNetCore.Components",
"Microsoft.AspNetCore.Components.Analyzers",
"Microsoft.AspNetCore.Components.Authorization",
"Microsoft.AspNetCore.Components.Forms",
"Microsoft.AspNetCore.Components.Server",
"Microsoft.AspNetCore.Components.Web",
"Microsoft.AspNetCore.ConcurrencyLimiter",
"Microsoft.AspNetCore.Connections.Abstractions",
"Microsoft.AspNetCore.CookiePolicy",
"Microsoft.AspNetCore.Cors",
"Microsoft.AspNetCore.Cryptography.Internal",
"Microsoft.AspNetCore.Cryptography.KeyDerivation",
"Microsoft.AspNetCore.DataProtection",
"Microsoft.AspNetCore.DataProtection.Abstractions",
"Microsoft.AspNetCore.DataProtection.AzureKeyVault",
"Microsoft.AspNetCore.DataProtection.AzureStorage",
"Microsoft.AspNetCore.DataProtection.EntityFrameworkCore",
"Microsoft.AspNetCore.DataProtection.Extensions",
"Microsoft.AspNetCore.DataProtection.StackExchangeRedis",
"Microsoft.AspNetCore.DeveloperCertificates.XPlat",
"Microsoft.AspNetCore.Diagnostics",
"Microsoft.AspNetCore.Diagnostics.Abstractions",
"Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore",
"Microsoft.AspNetCore.Diagnostics.HealthChecks",
"Microsoft.AspNetCore.HeaderPropagation",
"Microsoft.AspNetCore.HostFiltering",
"Microsoft.AspNetCore.Hosting",
"Microsoft.AspNetCore.Hosting.Abstractions",
"Microsoft.AspNetCore.Hosting.Server.Abstractions",
"Microsoft.AspNetCore.Hosting.WindowsServices",
"Microsoft.AspNetCore.Html.Abstractions",
"Microsoft.AspNetCore.Http",
"Microsoft.AspNetCore.Http.Abstractions",
"Microsoft.AspNetCore.Http.Connections",
"Microsoft.AspNetCore.Http.Connections.Client",
"Microsoft.AspNetCore.Http.Connections.Common",
"Microsoft.AspNetCore.Http.Extensions",
"Microsoft.AspNetCore.Http.Features",
"Microsoft.AspNetCore.HttpOverrides",
"Microsoft.AspNetCore.HttpsPolicy",
"Microsoft.AspNetCore.Identity",
"Microsoft.AspNetCore.Identity.EntityFrameworkCore",
"Microsoft.AspNetCore.Identity.UI",
"Microsoft.AspNetCore.JsonPatch",
"Microsoft.AspNetCore.Localization",
"Microsoft.AspNetCore.Localization.Routing",
"Microsoft.AspNetCore.Metadata",
"Microsoft.AspNetCore.MiddlewareAnalysis",
"Microsoft.AspNetCore.Mvc",
"Microsoft.AspNetCore.Mvc.Abstractions",
"Microsoft.AspNetCore.Mvc.Analyzers",
"Microsoft.AspNetCore.Mvc.Api.Analyzers",
"Microsoft.AspNetCore.Mvc.ApiExplorer",
"Microsoft.AspNetCore.Mvc.Core",
"Microsoft.AspNetCore.Mvc.Cors",
"Microsoft.AspNetCore.Mvc.DataAnnotations",
"Microsoft.AspNetCore.Mvc.Formatters.Json",
"Microsoft.AspNetCore.Mvc.Formatters.Xml",
"Microsoft.AspNetCore.Mvc.Localization",
"Microsoft.AspNetCore.Mvc.NewtonsoftJson",
"Microsoft.AspNetCore.Mvc.Razor",
"Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation",
"Microsoft.AspNetCore.Mvc.RazorPages",
"Microsoft.AspNetCore.Mvc.TagHelpers",
"Microsoft.AspNetCore.Mvc.ViewFeatures",
"Microsoft.AspNetCore.NodeServices",
"Microsoft.AspNetCore.Owin",
"Microsoft.AspNetCore.Razor",
"Microsoft.AspNetCore.Razor.Runtime",
"Microsoft.AspNetCore.ResponseCaching",
"Microsoft.AspNetCore.ResponseCaching.Abstractions",
"Microsoft.AspNetCore.ResponseCompression",
"Microsoft.AspNetCore.Rewrite",
"Microsoft.AspNetCore.Routing",
"Microsoft.AspNetCore.Routing.Abstractions",
"Microsoft.AspNetCore.Server.HttpSys",
"Microsoft.AspNetCore.Server.IIS",
"Microsoft.AspNetCore.Server.IISIntegration",
"Microsoft.AspNetCore.Server.Kestrel",
"Microsoft.AspNetCore.Server.Kestrel.Core",
"Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv",
"Microsoft.AspNetCore.Server.Kestrel.Transport.Quic",
"Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets",
"Microsoft.AspNetCore.Session",
"Microsoft.AspNetCore.SignalR",
"Microsoft.AspNetCore.SignalR.Client",
"Microsoft.AspNetCore.SignalR.Client.Core",
"Microsoft.AspNetCore.SignalR.Common",
"Microsoft.AspNetCore.SignalR.Core",
"Microsoft.AspNetCore.SignalR.Protocols.Json",
"Microsoft.AspNetCore.SignalR.Protocols.MessagePack",
"Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson",
"Microsoft.AspNetCore.SignalR.StackExchangeRedis",
"Microsoft.AspNetCore.SpaServices",
"Microsoft.AspNetCore.SpaServices.Extensions",
"Microsoft.AspNetCore.StaticFiles",
"Microsoft.AspNetCore.WebSockets",
"Microsoft.AspNetCore.WebUtilities",
"Microsoft.Build",
"Microsoft.Build.Framework",
"Microsoft.Build.Utilities.Core",
"Microsoft.CSharp",
"Microsoft.Data.Analysis",
"Microsoft.Diagnostics.Tracing.EventSource",
"Microsoft.DotNet.Analyzers.Async",
"Microsoft.DotNet.Arcade.Sdk",
"Microsoft.Experimental.Collections",
"Microsoft.Extensions.ActivatorUtilities.Sources",
"Microsoft.Extensions.ApiDescription.Client",
"Microsoft.Extensions.ApiDescription.Server",
"Microsoft.Extensions.Caching.Abstractions",
"Microsoft.Extensions.Caching.Memory",
"Microsoft.Extensions.Caching.SqlServer",
"Microsoft.Extensions.Caching.StackExchangeRedis",
"Microsoft.Extensions.CommandLineUtils.Sources",
"Microsoft.Extensions.Configuration",
"Microsoft.Extensions.Configuration.Abstractions",
"Microsoft.Extensions.Configuration.AzureKeyVault",
"Microsoft.Extensions.Configuration.Binder",
"Microsoft.Extensions.Configuration.CommandLine",
"Microsoft.Extensions.Configuration.EnvironmentVariables",
"Microsoft.Extensions.Configuration.FileExtensions",
"Microsoft.Extensions.Configuration.Ini",
"Microsoft.Extensions.Configuration.Json",
"Microsoft.Extensions.Configuration.KeyPerFile",
"Microsoft.Extensions.Configuration.NewtonsoftJson",
"Microsoft.Extensions.Configuration.UserSecrets",
"Microsoft.Extensions.Configuration.Xml",
"Microsoft.Extensions.DependencyInjection",
"Microsoft.Extensions.DependencyInjection.Abstractions",
"Microsoft.Extensions.DiagnosticAdapter",
"Microsoft.Extensions.Diagnostics.HealthChecks",
"Microsoft.Extensions.Diagnostics.HealthChecks.Abstractions",
"Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore",
"Microsoft.Extensions.FileProviders.Abstractions",
"Microsoft.Extensions.FileProviders.Composite",
"Microsoft.Extensions.FileProviders.Embedded",
"Microsoft.Extensions.FileProviders.Physical",
"Microsoft.Extensions.FileSystemGlobbing",
"Microsoft.Extensions.HashCodeCombiner.Sources",
"Microsoft.Extensions.HostFactoryResolver.Sources",
"Microsoft.Extensions.Hosting",
"Microsoft.Extensions.Hosting.Abstractions",
"Microsoft.Extensions.Hosting.Systemd",
"Microsoft.Extensions.Hosting.WindowsServices",
"Microsoft.Extensions.Http",
"Microsoft.Extensions.Http.Polly",
"Microsoft.Extensions.Identity.Core",
"Microsoft.Extensions.Identity.Stores",
"Microsoft.Extensions.Localization",
"Microsoft.Extensions.Localization.Abstractions",
"Microsoft.Extensions.Logging",
"Microsoft.Extensions.Logging.Abstractions",
"Microsoft.Extensions.Logging.Analyzers",
"Microsoft.Extensions.Logging.AzureAppServices",
"Microsoft.Extensions.Logging.Configuration",
"Microsoft.Extensions.Logging.Console",
"Microsoft.Extensions.Logging.Debug",
"Microsoft.Extensions.Logging.EventLog",
"Microsoft.Extensions.Logging.EventSource",
"Microsoft.Extensions.Logging.TraceSource",
"Microsoft.Extensions.ML",
"Microsoft.Extensions.NonCapturingTimer.Sources",
"Microsoft.Extensions.ObjectPool",
"Microsoft.Extensions.Options",
"Microsoft.Extensions.Options.ConfigurationExtensions",
"Microsoft.Extensions.Options.DataAnnotations",
"Microsoft.Extensions.ParameterDefaultValue.Sources",
"Microsoft.Extensions.Primitives",
"Microsoft.Extensions.TypeNameHelper.Sources",
"Microsoft.Extensions.ValueStopwatch.Sources",
"Microsoft.Extensions.WebEncoders",
"Microsoft.IO.Redist",
"Microsoft.JSInterop",
"Microsoft.ML.AutoML",
"Microsoft.ML.CodeGenerator",
"Microsoft.ML.Core",
"Microsoft.ML.CpuMath",
"Microsoft.ML.Data",
"Microsoft.ML.DataView",
"Microsoft.ML.DnnImageFeaturizer.AlexNet",
"Microsoft.ML.DnnImageFeaturizer.ResNet101",
"Microsoft.ML.DnnImageFeaturizer.ResNet18",
"Microsoft.ML.DnnImageFeaturizer.ResNet50",
"Microsoft.ML.Ensemble",
"Microsoft.ML.EntryPoints",
"Microsoft.ML.Experimental",
"Microsoft.ML.FastTree",
"Microsoft.ML.Featurizers",
"Microsoft.ML.ImageAnalytics",
"Microsoft.ML.KMeansClustering",
"Microsoft.ML.LightGbm",
"Microsoft.ML.Maml",
"Microsoft.ML.Mkl.Components",
"Microsoft.ML.OnnxConverter",
"Microsoft.ML.OnnxTransformer",
"Microsoft.ML.Parquet",
"Microsoft.ML.PCA",
"Microsoft.ML.Recommender",
"Microsoft.ML.ResultProcessor",
"Microsoft.ML.StandardTrainers",
"Microsoft.ML.Sweeper",
"Microsoft.ML.TensorFlow",
"Microsoft.ML.TimeSeries",
"Microsoft.ML.Transforms",
"Microsoft.ML.Vision",
"Microsoft.Net.Http.Headers",
"Microsoft.SourceLink.AzureRepos.Git",
"Microsoft.SourceLink.Common",
"Microsoft.SourceLink.GitHub",
"Microsoft.VisualBasic",
"Microsoft.VisualBasic.Core",
"Microsoft.Web.Xdt.Extensions",
"Microsoft.Win32.Primitives",
"Microsoft.Win32.Registry",
"Microsoft.Win32.Registry.AccessControl",
"Microsoft.Win32.SystemEvents",
"MML",
"Mono.WebAssembly.Interop",
"MSBuild",
"MSBuild.Bootstrap",
"MSBuild.Engine.Corext",
"MSBuild.VSSetup",
"MSBuildFiles",
"MSBuildItems",
"MSBuildProperties",
"MSBuildTargets",
"OneWire",
"PresentationCore",
"PresentationFramework",
"PresentationUI",
"RadioReceiver",
"RadioTransmitter",
"ReachFramework",
"RGBLedMatrix",
"Rtc",
"Seesaw",
"SenseHat",
"ServoMotor",
"Sht3x",
"SocketCan",
"SoftwarePwm",
"SoftwareSpi",
"System.AppContext",
"System.Azure.Experimental",
"System.Binary.Base64",
"System.CodeDom",
"System.Collections",
"System.Collections.Concurrent",
"System.Collections.Immutable",
"System.Collections.NonGeneric",
"System.Collections.Sequences",
"System.Collections.Specialized",
"System.ComponentModel",
"System.ComponentModel.Annotations",
"System.ComponentModel.Composition",
"System.ComponentModel.Composition.Registration",
"System.ComponentModel.EventBasedAsync",
"System.ComponentModel.Primitives",
"System.ComponentModel.TypeConverter",
"System.Composition.AttributedModel",
"System.Composition.Convention",
"System.Composition.Hosting",
"System.Composition.Runtime",
"System.Composition.TypedParts",
"System.Configuration.ConfigurationManager",
"System.Console",
"System.Data.Common",
"System.Data.DataSetExtensions",
"System.Data.Odbc",
"System.Data.OleDb",
"System.Design",
"System.Device.Gpio",
"System.DirectoryServices",
"System.DirectoryServices.AccountManagement",
"System.DirectoryServices.Protocols",
"System.Drawing",
"System.Drawing.Common",
"System.Drawing.Design",
"System.Drawing.Primitives",
"System.Dynamic.Runtime",
"System.Globalization.Calendars",
"System.Globalization.Extensions",
"System.IO",
"System.IO.Compression",
"System.IO.Compression.Brotli",
"System.IO.Compression.ZipFile",
"System.IO.FileSystem",
"System.IO.FileSystem.AccessControl",
"System.IO.FileSystem.DriveInfo",
"System.IO.FileSystem.Primitives",
"System.IO.FileSystem.Watcher",
"System.IO.FileSystem.Watcher.Polling",
"System.IO.IsolatedStorage",
"System.IO.MemoryMappedFiles",
"System.IO.Packaging",
"System.IO.Pipelines",
"System.IO.Pipes",
"System.IO.Pipes.AccessControl",
"System.IO.Ports",
"System.IO.UnmanagedMemoryStream",
"System.Linq",
"System.Linq.Expressions",
"System.Linq.Parallel",
"System.Linq.Queryable",
"System.Management",
"System.Memory",
"System.Memory.Polyfill",
"System.Net.Http",
"System.Net.Http.WinHttpHandler",
"System.Net.HttpListener",
"System.Net.Mail",
"System.Net.NameResolution",
"System.Net.NetworkInformation",
"System.Net.Ping",
"System.Net.Primitives",
"System.Net.Requests",
"System.Net.Security",
"System.Net.ServicePoint",
"System.Net.Sockets",
"System.Net.WebClient",
"System.Net.WebHeaderCollection",
"System.Net.WebProxy",
"System.Net.WebSockets",
"System.Net.WebSockets.Client",
"System.Net.WebSockets.WebSocketProtocol",
"System.Numerics.Experimental",
"System.Numerics.Tensors",
"System.Numerics.Vectors",
"System.ObjectModel",
"System.Private.CoreLib",
"System.Private.DataContractSerialization",
"System.Private.ServiceModel",
"System.Private.Uri",
"System.Private.Xml",
"System.Private.Xml.Linq",
"System.Reflection.Context",
"System.Reflection.DispatchProxy",
"System.Reflection.Emit.ILGeneration",
"System.Reflection.Emit.Lightweight",
"System.Reflection.Extensions",
"System.Reflection.Metadata",
"System.Reflection.Metadata.Cil",
"System.Reflection.MetadataLoadContext",
"System.Reflection.Primitives",
"System.Reflection.TypeExtensions",
"System.Reflection.TypeLoader",
"System.Resources.Extensions",
"System.Resources.Reader",
"System.Resources.ResourceManager",
"System.Resources.Writer",
"System.Runtime.Caching",
"System.Runtime.CompilerServices.VisualC",
"System.Runtime.Extensions",
"System.Runtime.Handles",
"System.Runtime.InteropServices",
"System.Runtime.InteropServices.RuntimeInformation",
"System.Runtime.InteropServices.WindowsRuntime",
"System.Runtime.Intrinsics",
"System.Runtime.Intrinsics.Experimental",
"System.Runtime.Loader",
"System.Runtime.Numerics",
"System.Runtime.Serialization.Formatters",
"System.Runtime.Serialization.Json",
"System.Runtime.Serialization.Primitives",
"System.Runtime.Serialization.Xml",
"System.Runtime.WindowsRuntime",
"System.Runtime.WindowsRuntime.UI.Xaml",
"System.Security.AccessControl",
"System.Security.Claims",
"System.Security.Cryptography.Algorithms",
"System.Security.Cryptography.Asn1.Experimental",
"System.Security.Cryptography.Cng",
"System.Security.Cryptography.Csp",
"System.Security.Cryptography.Encoding",
"System.Security.Cryptography.OpenSsl",
"System.Security.Cryptography.Pkcs",
"System.Security.Cryptography.Primitives",
"System.Security.Cryptography.ProtectedData",
"System.Security.Cryptography.X509Certificates",
"System.Security.Cryptography.Xml",
"System.Security.Permissions",
"System.Security.Principal",
"System.Security.Principal.Windows",
"System.Security.SecureString",
"System.ServiceModel.Duplex",
"System.ServiceModel.Http",
"System.ServiceModel.NetTcp",
"System.ServiceModel.Primitives",
"System.ServiceModel.Security",
"System.ServiceModel.Syndication",
"System.ServiceProcess.ServiceController",
"System.Text.CaseFolding",
"System.Text.Encoding",
"System.Text.Encoding.CodePages",
"System.Text.Encoding.Extensions",
"System.Text.Encodings.Web",
"System.Text.Encodings.Web.Utf8",
"System.Text.Formatting",
"System.Text.Formatting.Globalization",
"System.Text.Http",
"System.Text.Json",
"System.Text.Primitives",
"System.Text.RegularExpressions",
"System.Text.Utf8String",
"System.Threading",
"System.Threading.AccessControl",
"System.Threading.Channels",
"System.Threading.Overlapped",
"System.Threading.Tasks.Dataflow",
"System.Threading.Tasks.Extensions",
"System.Threading.Tasks.Parallel",
"System.Threading.Timer",
"System.Time",
"System.Transactions.Local",
"System.Utf8String.Experimental",
"System.ValueTuple",
"System.Web.HttpUtility",
"System.Windows.Controls.Ribbon",
"System.Windows.Extensions",
"System.Windows.Forms",
"System.Windows.Forms.Design",
"System.Windows.Forms.Design.Editors",
"System.Windows.Forms.Primitives",
"System.Windows.Input.Manipulations",
"System.Windows.Presentation",
"System.Xaml",
"System.Xml.ReaderWriter",
"System.Xml.XDocument",
"System.Xml.XmlDocument",
"System.Xml.XmlSerializer",
"System.Xml.XPath",
"System.Xml.XPath.XDocument",
"TypeScriptFiles",
"Units",
"WindowsBase",
"WindowsFormsIntegration",
"Xunit.NetCore.Extensions",

        };
        #endregion

    }
}