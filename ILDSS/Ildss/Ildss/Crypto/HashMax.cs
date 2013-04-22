using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Crypto
{
    class HashMax : IHash
    {
        public string ByteToString(byte[] array)
        {
            int i;
            string hs = "";

            for (i = 0; i < array.Length; i++)
            {
                hs += (String.Format("{0:X2}", array[i]));
            }
            return hs;
        }

        public string HashFile(string path)
        {
            try
            {
                var fi = new FileInfo(path);
                byte[] bytes = new byte[3072];
                string raw = "";
                string hash = "";
                var sha256 = new SHA256Managed();

                FileStream fs = System.IO.File.OpenRead(path);
                Logger.write("Hashing " + fi.Name + ", " + fi.Length);
                if (fi.Length > 3072)
                {
                    long middle = (long)Math.Floor((double)fi.Length / (double)2);
                    long last = fi.Length - 1024;

                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(bytes, 0, 1024);
                    fs.Seek(middle, SeekOrigin.Current);
                    fs.Read(bytes, 1024, 1024);
                    fs.Seek(last, SeekOrigin.Current);
                    fs.Read(bytes, 2048, 1024);

                    raw = ByteToString(bytes);

                    hash = ByteToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(raw))) + fi.Length.ToString();
                }
                else
                {
                    hash = ByteToString(sha256.ComputeHash(fs)) + fi.Length.ToString();  
                }

                Logger.write("NewHash: " + hash);
                
                fs.Close();
                return hash;
            }
            catch (FileNotFoundException)
            {
                // file not found
                Logger.write("Error, file not found");
            }
            catch (IOException ex)
            {
                // file could not be accessed
                // warn user and try again on OK
                Logger.write("Error, " + ex.Message);
            }
            return null;
        }

        public string HashString(string str)
        {
            var sha256 = new SHA256Managed();
            return ByteToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }
    }
}
