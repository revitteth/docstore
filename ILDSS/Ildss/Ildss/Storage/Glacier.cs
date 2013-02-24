using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Storage
{
    public class Glacier : IStorage
    {
        public bool MoveToStorage(string path)
        {
            // move it
            return true;
        }
    }
}
