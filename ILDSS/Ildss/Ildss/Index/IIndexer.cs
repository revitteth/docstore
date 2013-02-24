using System;

namespace Ildss.Index
{
    public interface IIndexer
    {
        void IndexFile(string path);
        void IndexFiles(string path);
    }
}
