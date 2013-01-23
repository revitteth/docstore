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
        private HashSet<IEvent> _distinctEventList = new HashSet<IEvent>();

        public void AddEvent(IEvent securityEvent)
        {
            _eventList.Add(securityEvent);
        }

        public void DeDuplicate()
        {
            _distinctEventList = new HashSet<IEvent>(_eventList.Distinct().ToList());
        }

        public void WriteToDB()
        {
            // write entire queue to db - or to comunal list?
        }

        public void PrintQueue()
        {
            DeDuplicate();

            foreach (IEvent securityEvent in _distinctEventList)
            {
                securityEvent.PrintEvent();
            }
        }
    }
}
