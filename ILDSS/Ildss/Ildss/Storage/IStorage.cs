using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Storage
{
    public interface IStorage
    {
        void Store();
        void StoreAll();
        void Retrieve();
    }
}
