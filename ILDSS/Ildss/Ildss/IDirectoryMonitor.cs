using System;

namespace Ildss
{
    interface IDirectoryMonitor
    {
        void MonitorFileSystem(string path);
    }
}
