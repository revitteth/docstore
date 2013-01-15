using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Ildss
{
    class Indexer
    {
        private int filesIndexed    { get; set; }
        private int totalFiles { get; set; }
        
        public void IndexFiles(string path)
        {
            filesIndexed = 0;
            totalFiles = System.IO.Directory.GetFiles(path, "*", SearchOption.AllDirectories).Count();

            if (System.IO.File.Exists(path))
            {
                IndexFile(path);
            }
            else if (System.IO.Directory.Exists(path))
            {
                IndexDirectory(path);
            }
        }

        // Recursively index subdirectories
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
            FileInfo fi = new FileInfo(path);
            ++filesIndexed;

            // Hash File
            Hash h = new Hash();
            string fileHash = h.HashFile(fi.FullName);

            // Insert File into Index Database
            using (FileIndexContainer fic = new FileIndexContainer())
            {

                var result = from documents in fic.Documents
                             where documents.DocumentHash == fileHash
                             select documents;

               // New Document
                Document doc = new Document()
                {
                    DocumentHash = fileHash,
                    size = fi.Length,
                };

                // New Path
                DocPath dp = new DocPath()
                {
                    path = fi.FullName
                };

                // If Document isn't Duplicate
                if (result.Count() == 0)
                {
                    fic.Documents.Add(doc);
                    doc.DocPaths.Add(dp);   //page 267/8 in Entity framework 4.0 recipes
                }
                else 
                // Document is duplicate
                {
                    DocPath d2 = new DocPath()
                    {
                        path = fi.FullName,
                        DocumentDocumentHash = fileHash
                    };
                    fic.DocPaths.Add(d2);
                }
                fic.SaveChanges();
            }

            Console.WriteLine("Saved " + fi.FullName + " to database");
        }

    }
}
