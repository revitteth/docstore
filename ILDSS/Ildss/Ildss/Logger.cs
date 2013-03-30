using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    class Logger
    {
        private IList<string> _log;

        public Logger()
        {

        }

        public void write(string line)
        {
            _log.Add(line);
        }


        public void print()
        {
            foreach (var line in _log)
            {
                Console.WriteLine(line);
            }
        }

        public void save()
        {

         // dump it into a file
        }
    }
}
