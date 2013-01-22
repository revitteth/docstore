using System;

namespace Ildss
{
    interface IEventQueue
    {
        void AddEvent(DocEvent de);
        void EventQueueToDb();
        void PrintEvents();
    }
}
