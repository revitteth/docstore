using Ildss.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Index
{
    public interface IReader
    {
        List<Tuple<string, string, DateTime>> GetFilesForIncrementalBackup();
        List<Document> GetUnusedDocumentsForLocalDeletion();
        List<string> GetUnusedFilesForLocalDeletion();
    }
}
