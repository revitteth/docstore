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
            foreach (var doc in fic.Documents.Where(i => i.Status == Enums.DocStatus.Indexed))
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

        public List<Document> GetUnusedDocumentsForLocalDeletion()
        {
            var unused = FindUnusedDocuments();
            return unused;
        }

        // Intelligence logic
        public List<string> GetUnusedFilesForLocalDeletion()
        {
            var unused = FindUnusedDocuments();
            List<string> unusedFiles = new List<string>();
            foreach (var un in unused)
            {
                foreach(var path in un.DocPaths)
                {
                    unusedFiles.Add(path.Path);   
                }
            }
            return unusedFiles;
        }

        private List<Document> FindUnusedDocuments()
        {
            DateTime now = DateTime.Now;
            List<Document> unused = new List<Document>();

            var fic = KernelFactory.Instance.Get<IFileIndexContext>();

            var documents = fic.Documents.Where(i => i.Status == Enums.DocStatus.Current);

            foreach (var doc in documents)
            {
                Logger.Write("Document " + doc.DocPaths.First().Path);
                // Size constraint is not met - search for documents older than a certain age to delete
                // find most recent event - if within last x then ignore else add document to list
                if (doc.DocEvents.Any(i => i.Time > (now - Settings.Default.MaxDormancy)))
                {
                    // document has been used recently so ignore it
                    Logger.Write("Used recently");
                }
                else
                {
                    Logger.Write("Unused - " + doc.DocumentId);
                    unused.Add(doc);
                }
            }

            // order oldest to newest
            unused.OrderBy(i => i.DocEvents.OrderBy(j => j.Time));

            long accumulator = 0;

            var toBeDeleted = new List<Document>();

            foreach (var u in unused)
            {
                // add to total until quota met
                if (accumulator > Settings.Default.TargetDiskUtil)
                {
                    break;
                }
                else
                {
                    Logger.Write("Actually deleting - " + u.DocPaths.First().Path);
                    toBeDeleted.Add(u);
                }
            }

            return toBeDeleted;

            // go through events for each document 
            // use evidence in research to retrieve the ones which are under utilised

            // if hard drive space target - get files in order of increasing usage until quota met
            // if time based target - get files with no events since x time
            // if based on decision - find all files marked for removal (this may not be necessary to implement for project) 
        }

        private DocEvent GetLastWriteEvent(Document doc)
        {
            return doc.DocEvents.OrderByDescending(i => i.Time).First(j => j.Type == Enums.EventType.Write);
        }
    }
}
