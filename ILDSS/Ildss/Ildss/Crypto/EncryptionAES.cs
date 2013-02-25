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
        private AesManaged _aes = null;
        private bool _compress = false;
        private string _inFile;
        private string _outFile;
        private string _extension = ".e";
        private byte[] _key;
        private byte[] _iv;
        

        public EncryptionAES()
        {
            _aes = new AesManaged();
            _key = _aes.Key;
            _iv = _aes.IV;
        }

        public FileInfo EncryptFile(string inFile, string outFile, bool compress = false)
        {
            // Set up object instance
            _inFile = inFile;
            _outFile = outFile;
            _compress = compress;

            // Create streams
            Stream infs = null, outfs = null, cryptoStream = null;

            try
            {
                infs = new FileStream(_inFile, FileMode.Open);
                outfs = new FileStream(_outFile + _extension, FileMode.OpenOrCreate);
                cryptoStream = new CryptoStream(outfs, _aes.CreateEncryptor(), CryptoStreamMode.Write);
                infs.CopyTo(cryptoStream);
            }
            finally
            {
                if (cryptoStream != null)
                    cryptoStream.Close();
                if (outfs != null)
                    outfs.Close();
                if (infs != null)
                    infs.Close();

                // Maybe leave this if new instance of class each time!
                if (_aes != null)
                    _aes.Clear();
            }
            return new FileInfo(outFile);
        }

        public bool DecryptFile(string inFile, string outFile)
        {
            



            return true;
        }

    }
}
