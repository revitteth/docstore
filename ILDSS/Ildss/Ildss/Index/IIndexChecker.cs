using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Index
{
    interface IIndexChecker
    {
        void RespondToEvent(string path, string type, string oldpath = "");
        void MaintainDocuments();
    }
}
