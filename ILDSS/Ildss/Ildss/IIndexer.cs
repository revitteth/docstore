using System;
namespace Ildss
{
    public interface IIndexer
    {
        void IndexFile(string path);
        void IndexFiles(string path);
    }
}
