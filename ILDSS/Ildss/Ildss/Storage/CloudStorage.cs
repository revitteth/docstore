using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Storage
{
    class CloudStorage : IStorage
    {
        public CloudStorage()
        {
            // create bucket if it doesn't exist & set policies

        }

        public void StoreIncr()
        {
                        
        }

        public void StoreFull()
        {
            throw new NotImplementedException();
        }

        public void RetrieveIncr()
        {
            throw new NotImplementedException();
        }

        public void RetrieveFull()
        {
            throw new NotImplementedException();
        }
    }
}
