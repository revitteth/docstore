using System;

namespace Ildss
{
    interface IQueue
    {
        void AddEvent(IEvent newEvent);
        void PrintQueue();
    }
}
