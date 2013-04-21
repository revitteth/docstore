using Ildss.Models;
using System;
using System.Collections.Generic;
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
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"E:\Documents\GitHub\docstore\output.csv");

            string line = "";

            file.WriteLine("DocID, DocHash, Event Type, Event Time");

            foreach (var doc in fic.Documents)
            {
                foreach (var ev in doc.DocEvents)
                {
                    line += doc.DocumentId + "," + doc.DocumentHash + "," + ev.Type.ToString() + "," + ev.Time;
                    file.WriteLine(line);
                    line = "";
                }
            }
            file.Close();

            Logger.write("Finished Exporting CSV " + @"E:\Documents\GitHub\docstore\output.csv");

        }
    }
}
