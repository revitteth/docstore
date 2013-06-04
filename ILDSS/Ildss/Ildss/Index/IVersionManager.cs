using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Index
{
    interface IVersionManager
    {
        // pass a status and a document and this sets that status on the document 
        // or pass a path - find the corresponding document and update its status
        void AddVersion(Enums.DocStatus status, List<Tuple<Models.Document, string, DateTime>> documents);
        void AddVersion(Enums.DocStatus status, List<Tuple<string, string, DateTime>> paths);
        void AddVersion(Enums.DocStatus status, Tuple<Models.Document, string, DateTime> document);                 
        void AddVersion(Enums.DocStatus status, Tuple<string, string, DateTime> path);
    }
}
