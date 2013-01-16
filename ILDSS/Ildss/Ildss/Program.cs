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
