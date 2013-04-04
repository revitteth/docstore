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
using SharpShell.SharpContextMenu;
using SharpShell.Attributes;

using Ildss;
using Ildss.Properties;


namespace DocstoreShell
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class ContextMenu : SharpContextMenu
    {
        string thetext = "dunno";
        string ildssDirectory = Settings.WorkingDir;

        // possibly query main program for this at startup (as with the above)
        private IList<string> _ignoredFiles = Settings.IgnoredExtensions;
        ContextMenuStrip MenuStrip = new ContextMenuStrip();

        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override System.Windows.Forms.ContextMenuStrip CreateMenu()
        {
            var selectedPaths = SelectedItemPaths.ToList();

            var selectedPath = selectedPaths.FirstOrDefault();

            if (selectedPaths.Count() > 1)
            {
                // just display link to back them all up (if needs be)
            }
            else
            {
                if (selectedPath.Contains(ildssDirectory) & !_ignoredFiles.Any(selectedPath.Contains))
                {
                    // work out if file or directory
                    if ((File.GetAttributes(selectedPath) == FileAttributes.Directory) | selectedPath == ildssDirectory)
                    {
                        thetext = selectedPath;
                        addMenuItem("Directory");
                    }
                    else
                    {
                        thetext = selectedPath;
                        addMenuItem("File");
                    }

                    
                    return MenuStrip;
                }
            }
            return null;
        }

        public void addMenuItem(string text)
        {
            var menuItem = new ToolStripMenuItem { Text = text, ToolTipText = "yesss geoff" }; //Image = Image.FromFile(@"..\..\cloud.png") };
            menuItem.Click += (sender, args) => Geoff();
            MenuStrip.Items.Add(menuItem);
        }

        public void Geoff()
        {
            MessageBox.Show(thetext);
        }
    }
}
