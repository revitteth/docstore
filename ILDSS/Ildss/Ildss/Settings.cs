using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    public static class Settings
    {
        // File extensions to be ignored
        public static IList<string> IgnoredExtensions = new List<string> { ".tmp", ".TMP", ".gz", ".ini", "~$", ".ildss" };
        public static string WorkingDir = @"E:\Documents\GitHub\docstore\TestDir"; //@"C:\Users\Max\Documents\GitHub\docstore\TestDir";
        public static string StorageDir = @"E:\Documents\GitHub\docstore\StorageDir"; //@"C:\Users\Max\Documents\GitHub\docstore\StorageDir";
        public static bool FirstRun = true;
        public static int IndexInterval = 20000;
  
        // enumerated variables
        public enum DocStatus { Indexed, Current, Archived };
        public enum EventType { Read, Write, Create, Rename };
        public enum BackupType { Incremental, Full };
    }
}
