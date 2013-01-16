using System;
namespace Ildss
{
    interface IHash
    {
        string ByteToString(byte[] array);
        string HashFile(string path);
        string HashString(string str);
    }
}
