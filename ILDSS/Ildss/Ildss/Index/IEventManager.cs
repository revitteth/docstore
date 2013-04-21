using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss.Index
{
    public interface IEventManager
    {
        void AddEvent(FSEvent Eve);
    }
}
