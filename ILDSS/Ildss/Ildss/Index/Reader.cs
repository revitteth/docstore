using Ildss.Models;
using Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Index
{
    public class Reader : IReader
    {
        public Reader()
        {
            // instantiate the reader
        }

        public List<Tuple<string,string, DateTime>> GetFilesForIncrementalBackup()
        {
            var incFiles = new List<Tuple<string, string, DateTime>>();
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            foreach (var doc in fic.Documents.Where(i => i.Status == Settings.DocStatus.Indexed))
            {
                // Generate the unique version name + get the path of the file to be uploaded
                var ev = GetLastWriteEvent(doc);

                var temp = new Tuple<string, string, DateTime>(doc.DocPaths.First().Path, doc.DocumentHash, ev.Time);
                incFiles.Add(temp);
            }

            return incFiles;
        }

        public List<string> GetFilesForFullBackup()
        {
            var allFiles = new List<string>();
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            foreach (var doc in fic.Documents)
            {
                allFiles.Add(doc.DocPaths.First().Path);
            }
            return allFiles;  
        }

        // REMEMBER IT IS DOING 30 SECOND UNUSED TEST CONDITION
        public List<string> GetUnusedFilesForLocalDeletion()
        {
            var unused = FindUnusedDocuments();

            List<string> files = new List<string>();
            foreach (var doc in unused)
            {
                files.Add(doc.DocPaths.First().Path);   
            }
            return files;
        }

        private List<Document> FindUnusedDocuments()
        {
            // get size/tim/both constraints from settings
            var util = Settings.TargetDiskUtilisation;
            var age = Settings.TargetDocumentMaxAge;

            DateTime from = DateTime.Now;

            List<Document> unused = new List<Document>();

            var fic = KernelFactory.Instance.Get<IFileIndexContext>();

            long sizeaccum = 0;

            var documents = fic.Documents.OrderByDescending(i => i.Status == Settings.DocStatus.Current);

            foreach (var doc in documents)
            {
                // check time constraint (if there is one e.g. 3 months)
                // find most recent event - if within last x then ignore else add document to list
                if (doc.DocEvents.Any(i => i.Time > (from - age)))
                {
                    // document has been used recently so ignore it
                }
                else
                {
                    if (doc.Size + sizeaccum > util)
                    { 
                        break; 
                    }
                    else
                    {
                        unused.Add(doc);
                        sizeaccum += doc.Size;
                        Logger.Write(sizeaccum.ToString() + " sizeaccum");
                    }
                }
            }

            return unused;

            // go through events for each document 
            // use evidence in research to retrieve the ones which are under utilised

            // if hard drive space target - get files in order of increasing usage until quota met
            // if time based target - get files with no events since x time
            // if based on decision - find all files marked for removal (this may not be necessary to implement for project) 
        }

        private DocEvent GetLastWriteEvent(Document doc)
        {
            return doc.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Settings.EventType.Write);
        }
    }
}
