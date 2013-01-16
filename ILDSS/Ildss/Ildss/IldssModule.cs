using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;

namespace Ildss
{
    class IldssModule : NinjectModule
    {
        public override void Load()
        {
            Console.WriteLine("im being called");
            this.Bind<IEventQueue>().To<SillyQueue>();
        }
    }
}
