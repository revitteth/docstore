using System;

namespace Ildss
{
    interface IEventQueue
    {
        void AddEvent(DocEvent de);
        void DetectOfficeFiles();
        void EventQueueToDb();
        void PrintEvents();
    }
}
