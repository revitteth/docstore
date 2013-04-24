using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ildss.Storage;

namespace Ildss.Index
{
    public interface IEventManager
    {
        void AddEvent(FSEvent Eve);
        bool IndexRequired { get; set; }
    }
}
