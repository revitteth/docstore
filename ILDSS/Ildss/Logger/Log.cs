using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log
{
    /// <summary>
    /// <para>
    /// Generic logging class for collecting information and outputting
    /// to a log file.
    /// </para>
    /// <para>
    /// Outputs the time and class of where the logging operation was called from
    /// along with a custom string passed in.
    /// </para>
    /// </summary>
    public static class Logger
    {
        private static string _line;
        public static string _logPath { get; set; }

        public static void Write(string line)
        {
            string where = new StackFrame(1).GetMethod().ReflectedType.ToString().Replace("Ildss.", "");
            if (where.Contains('+'))
            {
                where = where.Substring(0, where.LastIndexOf('+'));
            }
            where = where.PadRight(30, ' ');
            _line = (DateTime.Now.ToString("hh:mm:ss:fff") + " " + where + " " + line);
            Save();
        }

        public static void Save()
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
