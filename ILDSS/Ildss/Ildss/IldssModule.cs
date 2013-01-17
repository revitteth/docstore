using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;

namespace Ildss
{
    public class IldssModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IEventQueue>().To<EventQueue>().InSingletonScope();
            this.Bind<IIndexer>().To<Indexer>();
            this.Bind<IHash>().To<Hash>();
            this.Bind<IFileIndexContext>().To<FileIndexContext>().InSingletonScope(); // may need to re-implement in the implementation file after changes
            this.Bind<IDirectoryMonitor>().To<DirectoryMonitor>().InSingletonScope();
        }
    }
}
