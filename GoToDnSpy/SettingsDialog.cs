using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace GoToDnSpy
{
    [Guid(GoToDnSpyPackage.PackageGuidString)]
    public class SettingsDialog : DialogPage
    {
        [Category("GoTo dnSpy")]
        [DisplayName("dnSpy path")]
        [Description("Path to dnSpy.exe. Example: C:\\dnSpy\\dnSpy.exe")]
        public string DnSpyPath { get; set;}
    }
}
