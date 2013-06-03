using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Index
{
    interface IStatusChanger
    {
        // pass a status and a document and this sets that status on the document 
        // or pass a path - find the corresponding document and update its status
        void UpdateStatus(Settings.DocStatus status, Models.Document document);
        void UpdateStatus(Settings.DocStatus status, List<Models.Document> documents);
        void UpdateStatus(Settings.DocStatus status, string path);
        void UpdateStatus(Settings.DocStatus status, List<string> paths);
    }
}
