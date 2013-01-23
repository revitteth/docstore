using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Security.Permissions;

namespace Ildss
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class HashSHA512 : IHash
    {

        private SHA512 sha512 { get; set; }
        private byte[] calculatedHash { get; set; }
        private string hashString { get; set; }


        public HashSHA512()
        {
            sha512 = new SHA512Managed();
            calculatedHash = null;
            hashString = null;
        }

        [STAThreadAttribute]
        public string HashFile(string path)
        {

            sha512 = new SHA512Managed();
            calculatedHash = null;
            hashString = null;
            FileInfo fi = new FileInfo(path);

            try
            {
                FileStream fs = System.IO.File.OpenRead(path);
                fs.Position = 0;
                calculatedHash = sha512.ComputeHash(fs);
                fs.Close();
                hashString = ByteToString(calculatedHash);
                return hashString;
            }
            catch (FileNotFoundException)
            {
                // file not found
                Console.WriteLine("error, file not found");
            }
            catch (IOException)
            {
                // file could not be accessed
                Console.WriteLine("error, file not accessible");
            }
            catch (UnauthorizedAccessException)
            {
                // no access permission
            }
            return null;

        }

        [STAThreadAttribute]
        public string HashString(string str)
        {
            return ByteToString(sha512.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        // Print the byte array in a readable format. 
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
    }
}
