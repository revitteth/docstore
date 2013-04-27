using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using SharpShell;
using SharpShell.Attributes;
using SharpShell.SharpIconOverlayHandler;
using SharpShell.Interop;

namespace IldssShell
{
    [ComVisible(true)]
    class Overlay : SharpIconOverlayHandler
    {
        protected override int GetPriority()
        {
            //  The read only icon overlay is very low priority.
            return 0;
        }

        protected override bool CanShowOverlay(string path, FILE_ATTRIBUTE attributes)
        {
            return true;
        }

        protected override Icon GetOverlayIcon()
        {
            //  Return the read only icon.
            return new Icon("../../time.ico");
        }
    }
}
