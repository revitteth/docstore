using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ildss.Properties;

namespace Ildss.Index
{
    class FrequentIndexer : IIndexer
    {
        private IList<string> _ignoredFiles = KernelFactory.Instance.Get<ISettings>().ignoredExtensions;

        public void IndexFiles(string path)
        {
            Logger.write("Frequent Index Started");
            if (System.IO.File.Exists(path))
            {
                IndexFile(path);
            }
            else if (System.IO.Directory.Exists(path))
            {
                IndexDirectory(path);
            }
            Logger.write("Frequent Index Finished");
        }

        private void IndexDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = System.IO.Directory.GetFiles(targetDirectory);
            
            foreach (string fileName in fileEntries)
                IndexFile(fileName);
            // Recurse into subdirectories of this directory.
            
            string[] subdirectoryEntries = System.IO.Directory.GetDirectories(targetDirectory);
            
            foreach (string subdirectory in subdirectoryEntries)
                IndexDirectory(subdirectory);
        }

        public void IndexFile(string path)
        {
            var fi = new FileInfo(path);

            if (!_ignoredFiles.Any(fi.Name.Contains) & fi.Name.Contains("."))
            {
                KernelFactory.Instance.Get<ICollector>().Register(path);
            }
        }
    }
}
