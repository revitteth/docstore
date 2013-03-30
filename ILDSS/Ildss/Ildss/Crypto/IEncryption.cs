using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Ildss.Crypto
{
    interface IEncryption
    {
        FileInfo Encrypt(FileInfo file);
        FileInfo Decrypt(FileInfo file);
    }
}
