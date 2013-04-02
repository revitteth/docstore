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
        static private string _line;

        static public void write(string line)
        {
            string where = new StackFrame(1).GetMethod().ReflectedType.ToString().Replace("Ildss.", "");
            if (where.Contains('+'))
            {
                where = where.Substring(0, where.LastIndexOf('+'));
            }
            where = where.PadRight(30, ' ');
            _line = (DateTime.Now.ToString("hh:mm:ss") + " " + where + " " + line);
            save();
        }

        static public void save()
        {
            StreamWriter logFile;

            if (!File.Exists("../../../../../log.txt"))
            {
                logFile = new StreamWriter("../../../../../log.txt");
            }
            else
            {
                logFile = File.AppendText("../../../../../log.txt");
            }
            
            logFile.WriteLine(_line);
            
            logFile.Close();
        }
    }
}
