using System;
namespace Ildss
{
    interface IIndexer
    {
        void IndexFile(string path);
        void IndexFiles(string path);
        void CheckIndex();
    }
}
