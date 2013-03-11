using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ildss.Compression;
using Ildss.Crypto;

namespace Ildss.Storage
{
    public class HardDrive : IStorage
    {
        public bool MoveToStorage(string path, string hash)
        {
            try
            {
                var fi = new FileInfo(path);
                var storageDir = Properties.Settings.Default.storageDir;

                // zip it
                var compFi = CompressionGZIP.Compress(fi);
                Console.WriteLine(compFi.FullName);
                // copy file to the storage path (no overwrite)
                File.Copy(compFi.FullName, Path.Combine(storageDir, hash), false);
                File.Delete(compFi.FullName);
                File.Delete(fi.FullName);
                Console.WriteLine("Succes, " + fi.Name + " was compressed, and moved");
                Console.WriteLine("New path: " + Path.Combine(storageDir, hash));
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine("File already exists");
                // in this case - do we do incremental?
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("File upload failure");
                return false;
            }
        }

        public bool RetrieveFromStorage(string path, string hash)
        {
            // find the file by hash
            var storageDir = Properties.Settings.Default.storageDir;
            var fi = new FileInfo(Path.Combine(storageDir, hash));
            Console.WriteLine("Retrieve file from: " + fi.FullName);

            // decompress it and move it back
            var decompFi = CompressionGZIP.Decompress(fi);
            Console.WriteLine(decompFi.FullName);
            File.Copy(decompFi.FullName, path);
            // delete the decompressed temp file (in glacier this will be much harder to do!)
            //File.Delete(decompFi.FullName);
            // get the last read/write times from events table
            // set them

            // present all paths - ask which location the user wants to put the file back in (check boxes)

            return true;
        }
    }
}
