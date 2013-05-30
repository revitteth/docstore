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
        List<string> GetFilesForIncrementalBackup();
        List<string> GetFilesForFullBackup();
        List<string> GetUnusedFilesForLocalDeletion();
    }
}
