using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Storage
{
    public class HardDrive : IStorage
    {
        public bool MoveToStorage(string path)
        {
            try
            {
                var fi = new FileInfo(path);
                var newDir = fi.Directory.FullName.Replace(Properties.Settings.Default.directory, Properties.Settings.Default.storageDir);
                Console.WriteLine("NEW DIR: " + newDir);

                if (!System.IO.Directory.Exists(newDir))
                {
                    System.IO.Directory.CreateDirectory(newDir);
                }

                // copy file to the storage path
                File.Copy(fi.FullName, newDir + fi.Name, false);

                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine("File already exists");
                // in this case - do we do incremental?
                return false;
            }
            catch (Exception e)
                Console.WriteLine("File upload failure");
                return false;
            }
        }
    }
}
