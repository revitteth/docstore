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

using CloudInterface;

namespace Ildss
{
    public class IldssModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IHash>().To<HashMax>();
            this.Bind<IFileIndexContext>().To<FileIndexContext>();
            this.Bind<IMonitor>().To<DirectoryMonitor>().InSingletonScope();
            //this.Bind<IMonitor>().To<DiscoMonitor>().InSingletonScope();
            this.Bind<IEventManager>().To<IndexManager>().InSingletonScope().Named("Index");
            this.Bind<IStorage>().To<LocalStorage>().InSingletonScope().Named("Local");
            this.Bind<IStorage>().To<CloudStorage>().InSingletonScope().Named("Cloud");
            this.Bind<IReader>().To<Reader>();

            // from class libraries
            this.Bind<ICloudManager>().To<CloudManager>().InSingletonScope();
        }
    }
}
