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

            // Set up ninject
            var e = KernelFactory.Instance.Get<IEventQueue>();

            e.PrintEvents();
            
            if (Properties.Settings.Default.firstrun)
            {
                // Run setup GUI
                Application.Run(new Setup());
            }
            else
            {
                // Use ninject to load all classes required on their own threads?


                //PUT IT ON A SEPERATE THREAD!!!!!
                // need to stop this getting garbage collected. also need to make sure it runs first time 
                //DirectoryMonitor dm = new DirectoryMonitor(Properties.Settings.Default.directory);
                
                // Run dashboard GUI
                Application.Run(new Ildss());
            }
        }

    }
}
