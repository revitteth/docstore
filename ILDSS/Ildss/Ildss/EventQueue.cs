using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ildss
{
    public class EventQueue : IQueue
    {
        private List<IEvent> _eventList = new List<IEvent>();

        public void AddEvent(IEvent dbEvent)
        {
            _eventList.Add(dbEvent);
        }

        public void PrintQueue()
        {
            foreach (IEvent dbEvent in _eventList)
            {
                Console.WriteLine(dbEvent.ToString() + '\t' + '\t');
            }
        }
    }
}
