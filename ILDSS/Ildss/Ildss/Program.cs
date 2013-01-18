using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ninject;
using Ninject.Modules;

namespace Ildss
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Properties.Settings.Default.firstrun)
            {
                // Run setup GUI
                Application.Run(new Setup());
            }
            else
            {                
                // Run dashboard GUI
                Application.Run(new Ildss());
            }
        }

    }
}

/*        public override bool Equals(DocEvent de)
        {
            if ((type == de.type) && (path == de.path) && (old_path == de.old_path) && (name == de.name) && (old_name == de.old_name) && (last_access == de.last_access)
                && (last_write == de.last_write))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
*/