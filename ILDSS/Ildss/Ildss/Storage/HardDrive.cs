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
                var newDir = Properties.Settings.Default.storageDir;

                // zip it
                var compFi = CompressionGZIP.Compress(fi);
                Console.WriteLine(compFi.FullName);
                // copy file to the storage path
                File.Copy(compFi.FullName, Path.Combine(newDir, hash), false);
                //File.Delete(compFi.FullName);
                Console.WriteLine("Succes, " + fi.Name + " was compressed, and moved");
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
    }
}
