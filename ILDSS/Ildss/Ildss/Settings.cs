using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    public class Settings : ISettings
    {
        // File extensions to be ignored by file indexer
        public IList<string> ignoredExtensions { get; set; }

        // Constructor
        public Settings()
        {
            ignoredExtensions = new List<string> { ".tmp", ".TMP", ".gz", ".ini", ".ildss" };
        }
    
    }
}
