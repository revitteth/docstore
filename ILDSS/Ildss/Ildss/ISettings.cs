using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    public interface ISettings
    {
        IList<string> ignoredExtensions { get; }
    }
}
