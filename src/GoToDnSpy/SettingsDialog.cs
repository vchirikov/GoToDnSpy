using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GoToDnSpy
{
    [Guid(PackageGuids.guidGoToDnSpyPackageString)]
    public class SettingsDialog : DialogPage
    {
        [Category("GoTo dnSpy")]
        [DisplayName("dnSpy path")]
        [Description("Path to dnSpy.exe. Example: C:\\dnSpy\\dnSpy.exe")]
        public string DnSpyPath { get; set;}

        /// <summary>
        /// Mode was added recently in new version <see cref="DialogPage"/> ?
        /// So this is workaround for removing it from settings property grid
        /// </summary>
        [Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public uint Mode => 1337;
    }
}
