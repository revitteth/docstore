using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    static class Logger
    {
        static private List<string> _log = new List<string>();

        static public void write(string line)
        {
            _log.Add(DateTime.Now.ToString("hh:mm:ss") + "  " + new StackFrame(1).GetMethod().ReflectedType + "  " + line);
            save();
        }

        static public void print()
        {
            foreach (var line in _log)
            {
                Console.WriteLine(line);
            }
        }

        static public void save()
        {
            StreamWriter logFile;

            if (!File.Exists("log.txt"))
            {
                logFile = new StreamWriter("log.txt");
            }
            else
            {
                logFile = File.AppendText("log.txt");
            }

            foreach (var line in _log)
            {
                logFile.WriteLine(line);
            }

            logFile.Close();
            _log.Clear();
        }
    }
}
