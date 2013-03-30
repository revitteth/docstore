using System;
using System.Collections.Generic;
using System.IO;
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

        public void writePrint(string line)
        {
            _log.Add(line);
            Console.WriteLine(line);
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
            StreamWriter logFile = new StreamWriter("log-" + DateTime.Now.ToString() + ".txt");
            foreach (var line in _log)
            {
                logFile.WriteLine(line);
            }
            logFile.Close();
        }
    }
}
