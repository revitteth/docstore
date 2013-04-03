﻿using System;
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
        public static string WorkingDir = @"C:\Users\Max\Documents\GitHub\docstore\TestDir";
        public static string StorageDir = @"C:\Users\Max\Documents\GitHub\docstore\StorageDir";
        public static bool FirstRun = true;  
  
        // enumerated variables
        public enum DocStatus { Indexed, Current, Archived };
        public enum EventType { Read, Write };
        public enum BackupType { Incremental, Full };
    }
}
