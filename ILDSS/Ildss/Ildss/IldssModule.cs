using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;

using Ildss.Models;
using Ildss.Index;
using Ildss.Crypto;
using Ildss.Storage;

namespace Ildss
{
    public class IldssModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IIndexer>().To<InitialIndexer>().Named("Initial");
            this.Bind<IIndexer>().To<FrequentIndexer>().InSingletonScope().Named("Frequent");
            this.Bind<IIndexChecker>().To<FrequentChecker>();
            this.Bind<IHash>().To<HashSHA512>();
            this.Bind<IFileIndexContext>().To<FileIndexContext>();
            this.Bind<IMonitor>().To<DirectoryMonitor>().InSingletonScope();
            this.Bind<ICollector>().To<EventCollector>().InSingletonScope();
            this.Bind<IStorage>().To<LocalStorage>();
        }
    }
}
