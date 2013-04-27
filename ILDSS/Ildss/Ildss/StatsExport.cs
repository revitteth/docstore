using Ildss.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Log;

namespace Ildss
{
    public static class StatsExport
    {
        public static void Export()
        {
            //read the database into a CSV file
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();

            Logger.Write("Exporting CSV!");

            // open CSV
            StreamWriter file = new StreamWriter(Settings.getStorageDir() + @"\output.csv", false);

            string line = "";

            file.WriteLine("Doc Count, Path Count, Event Count, Duplicate Count");
            file.WriteLine(fic.Documents.Count() + "," + fic.DocPaths.Count() + "," + fic.DocEvents.Count() + "," + fic.Documents.Where(i => i.DocPaths.Count() > 1).Count());
            file.WriteLine("");

            string tempLine = "Duplicate DocumentId(s):";
            foreach (var dupe in fic.Documents.Where(i => i.DocPaths.Count() > 1))
            {
                tempLine += "," + dupe.DocumentId;
            }
            file.WriteLine(tempLine);
            file.WriteLine("");

            file.WriteLine("DocID, Size, DocHash, Path, Name, No. Reads, No. Writes");

            foreach (var doc in fic.Documents.OrderBy(i => i.DocumentId))
            {
                foreach (var path in fic.DocPaths.Where(i => i.DocumentId == doc.DocumentId).OrderBy(j => j.DocPathId))
                {
                    line += "\"" + doc.DocumentId + "\",\"" + doc.Size + "\",\"" + doc.DocumentHash + "\",\"" + path.Path + "\",\"" + path.Name + "\",\"" + doc.DocEvents.Where(i => i.Type == Settings.EventType.Read).Count() + "\",\"" + doc.DocEvents.Where(i => i.Type == Settings.EventType.Write).Count() + "\"";
                        file.WriteLine(line);
                        line = "";
                }
            }

            file.WriteLine("");

            file.WriteLine("DocID, Size, DocHash, Path, Name, Event Type, Event Time");

            foreach (var doc in fic.Documents.OrderBy(i => i.DocumentId))
            {
                foreach (var path in fic.DocPaths.Where(i => i.DocumentId == doc.DocumentId).OrderBy(j => j.DocPathId))
                {
                    foreach (var ev in doc.DocEvents.OrderBy(i => i.DocEventId))
                    {
                        line += "\"" + doc.DocumentId + "\",\"" + doc.Size + "\",\"" + doc.DocumentHash + "\",\"" + path.Path + "\",\"" + path.Name + "\",\"" + ev.Type.ToString() + "\",\"" + ev.Time + "\"";
                        file.WriteLine(line);
                        line = "";
                    }
                }
            }
            file.Close();

            Logger.Write("Finished Exporting CSV " + @"E:\Documents\GitHub\docstore\output.csv");

        }
    }
}
