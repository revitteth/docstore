using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using System.Security.Permissions;

namespace Ildss.Crypto
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class EncryptionAES : IEncryption
    {
 
        public EncryptionAES()
        {

        }

        public FileInfo Encrypt(FileInfo file)
        {
            // copy the file into a new name (+.ildss) then encrypt
            string encFilePath = file.DirectoryName + file.Name + ".ildss";
            file.CopyTo(encFilePath);
            FileInfo encFile = new FileInfo(encFilePath);
            File.Encrypt(encFilePath);
            return encFile;
        }

        public FileInfo Decrypt(FileInfo file)
        {
            // database interaction here or elsewhere?
            return file;
        }

    }
}
