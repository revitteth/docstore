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
            //this.Bind<IHash>().To<HashSHA512>();
            this.Bind<IHash>().To<HashMax>();
            this.Bind<IFileIndexContext>().To<FileIndexContext>();
            this.Bind<IMonitor>().To<DirectoryMonitor>().InSingletonScope();
            //this.Bind<IMonitor>().To<DiscoMonitor>().InSingletonScope();
            this.Bind<IEventManager>().To<IndexManager>().Named("Index");
            this.Bind<IEventManager>().To<BackupManager>().Named("Backup");
            this.Bind<IStorage>().To<LocalStorage>();
        }
    }
}
