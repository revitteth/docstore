using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


// TODO 
// Write files/directories to DB.
// Check if files/directories already exist in DB

namespace Ildss
{
    class Indexer
    {
        private int filesIndexed    { get; set; }
        private int totalFiles { get; set; }

        
        public void IndexFiles(string path)
        {
            filesIndexed = 0;
            totalFiles = System.IO.Directory.GetFileSystemEntries(path, "*", SearchOption.AllDirectories).Count();


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
            filesIndexed++;
            Console.WriteLine(fi.FullName + " " + filesIndexed + "/" + totalFiles);//((filesIndexed/totalFiles) * 100) + "%");
        }

    }
}
