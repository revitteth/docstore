using Ildss.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Storage
{
    public interface IStorage
    {
        Task StoreIncrAsync();
        void Retrieve(string key, string dest, Document doc);
        void RemoveUnusedDocumentsAsync();
    }
}
