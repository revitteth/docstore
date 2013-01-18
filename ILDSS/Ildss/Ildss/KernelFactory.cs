using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Ninject;

namespace Ildss
{
    public class KernelFactory
    {
        private static readonly KernelFactory _Instance = new KernelFactory();

        public static KernelFactory Instance
        {
            get
            {
                return _Instance;
            }
        }
        // Contains a single reference to each assembly's kernel
        public static Dictionary<String, IKernel> AssemblyKernelRegistry = new Dictionary<string, IKernel>();

        // Retrieves an instance of the Kernel
        public IKernel GetKernel()
        {

            //Current Assembly
            Assembly assemblyReference = this.GetType().Assembly;
            string assemblyKey = assemblyReference.FullName;

            //Instantiate a new kernel for this assembly if it doesn't exist
            if (!AssemblyKernelRegistry.ContainsKey(assemblyKey))
            {
                if (ValidateSubclasses())
                {
                    var kernel = new StandardKernel();
                    kernel.Load(assemblyReference);
                    AssemblyKernelRegistry.Add(assemblyKey, kernel);
                }
                else
                {
                    return null;
                }
            }
            return AssemblyKernelRegistry[assemblyKey];
        }

        // Convenience method to abstract away retrieval of the instance and kernel
        // simply to request a type. Now you can simply do KernelFactory.Get&lt;T&gt;();
        public T Get<T>()
        {
            return GetKernel().Get<T>();
        }

        public T Get<T>(string name)
        {
            return GetKernel().Get<T>(name);
        }


        // Verifies that subclasses of this class pass the minimum requirements
        // Boolean value indicating whether subclasses succeed or not</returns>
        private bool ValidateSubclasses()
        {
            try
            {
                var baseClassType = typeof(KernelFactory);
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.GetTypes().Where(type => type.IsSubclassOf(baseClassType)))
                    {
                        var instanceProperty = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                        if (instanceProperty == null)
                        {
                            throw new MissingMemberException();
                        }
                    }
                }
            }
            catch (AppDomainUnloadedException ex)
            {
                return false;
            }
            return true;
        }
    }
}
