using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    class EventCollector
    {
        public void CheckForEvents()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();

        }
    }
}
