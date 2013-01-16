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
            this.Bind<IFileIndexContainer>().To<FileIndexContainer>().InSingletonScope();
        }
    }
}
