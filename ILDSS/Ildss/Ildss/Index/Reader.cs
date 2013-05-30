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
    }
}
