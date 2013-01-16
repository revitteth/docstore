using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    class SillyQueue : IEventQueue
    {
        public void AddEvent(DocEvent de)
        {
            // do somat
            PrintEvents();
        }

        public void DetectOfficeFiles()
        {
            throw new NotImplementedException();
        }

        public void EventQueueToDb()
        {
            throw new NotImplementedException();
        }

        public void PrintEvents()
        {
            Console.WriteLine("silly events");
        }
    }
}
