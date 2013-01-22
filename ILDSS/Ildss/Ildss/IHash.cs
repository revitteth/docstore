using System;
namespace Ildss
{
    public interface IHash
    {
        string ByteToString(byte[] array);
        string HashFile(string path);
        string HashString(string str);
    }
}
