using Ildss.Models;
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

        public List<string> GetFilesForIncrementalBackup()
        {
            var incFiles = new List<string>();
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            foreach (var doc in fic.Documents.Where(i => i.Status == Settings.DocStatus.Indexed))
            {
                incFiles.Add(doc.DocPaths.First().Path);
                //doc.Status = Settings.DocStatus.Current;
                // THIS SHOULD BE DONE ON UPLOAD COMPLETED!
            }
            fic.SaveChanges();

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

            // is it possible to calculate them using code?
            // ENSURE that documents are not already archived - status must be set once deleted from local

            // fake timespan - 30 seconds for testing
            TimeSpan Mandem = new TimeSpan();
            Mandem = TimeSpan.FromSeconds(30);


            DateTime from = DateTime.Now;

            List<Document> unused = new List<Document>();

            var fic = KernelFactory.Instance.Get<IFileIndexContext>();

            var documents = fic.Documents.Where(i => i.Status == Settings.DocStatus.Current);

            foreach (var doc in documents)
            {
                // check time constraint (if there is one e.g. 3 months)
                // find most recent event - if within last x then ignore else add document to list
                if (doc.DocEvents.Any(i => i.Time > (from - Mandem)))
                {
                    // document has been used recently so ignore it
                }
                else
                {
                    unused.Add(doc);
                }
            }

            return unused;

            // go through events for each document 
            // use evidence in research to retrieve the ones which are under utilised

            // if hard drive space target - get files in order of increasing usage until quota met
            // if time based target - get files with no events since x time
            // if based on decision - find all files marked for removal (this may not be necessary to implement for project) 
        }
    }
}
