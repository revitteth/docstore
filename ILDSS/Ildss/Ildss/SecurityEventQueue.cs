using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    class SecurityEventQueue : IQueue
    {
        private List<IEvent> _eventList = new List<IEvent>();

        public void AddEvent(IEvent securityEvent)
        {
            _eventList.Add(securityEvent);
        }

        public void PrintQueue()
        {
            foreach (IEvent securityEvent in _eventList)
            {
                securityEvent.PrintEvent();
            }
        }
    }
}
