using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ildss.Properties;
using Ildss.Models;
using Ildss.Crypto;

namespace Ildss.Index
{
    class FrequentIndexer : IIndexer
    {
        private IList<string> _ignoredFiles = Settings.IgnoredExtensions;

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
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var hash = KernelFactory.Instance.Get<IHash>().HashFile(path);

            // need WAY more logic here!

            if (!_ignoredFiles.Any(fi.Name.Contains) & fi.Name.Contains("."))
            {
                if (!fic.Documents.Any(i => i.DocPaths.Any(j => j.Path == path)))
                {
                    Logger.write("TROUBLE");
                    var newPath = new DocPath() { Directory = fi.DirectoryName, Path = fi.FullName, Name = fi.Name };

                    if (fic.Documents.Any(i => i.DocumentHash == hash))
                    {
                        fic.Documents.FirstOrDefault(i => i.DocumentHash == hash).DocPaths.Add(newPath);
                    }
                    else
                    {
                        var newDoc = new Document() { DocumentHash = hash, Size = fi.Length, Status = Settings.DocStatus.Indexed };
                        newDoc.DocPaths.Add(newPath);
                        fic.Documents.Add(newDoc);
                    }
                    fic.SaveChanges();
                }
                else
                {
                    KernelFactory.Instance.Get<ICollector>().Register(path);
                }
            }
        }
    }
}
