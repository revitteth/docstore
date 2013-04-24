using Ildss.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    public static class StatsExport
    {
        public static void Export()
        {
            //read the database into a CSV file
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();

            Logger.write("Exporting CSV!");

            // open CSV
            StreamWriter file = new StreamWriter(Settings.getStorageDir() + @"\output.csv", false);

            string line = "";

            file.WriteLine("DocID, DocHash, Event Type, Event Time, Path1, Path2, Path3, Path4");

            foreach (var doc in fic.Documents)
            {
                foreach (var ev in doc.DocEvents)
                {
                    line += doc.DocumentId + "," + doc.DocumentHash + "," + ev.Type.ToString() + "," + ev.Time;
                    foreach (var path in fic.DocPaths)
                    {
                        line += path.Path + ",";
                    }
                    file.WriteLine(line);
                    line = "";
                }
            }
            file.Close();

            Logger.write("Finished Exporting CSV " + @"E:\Documents\GitHub\docstore\output.csv");

        }
    }
}
