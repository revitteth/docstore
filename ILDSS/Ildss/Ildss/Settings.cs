using Ildss.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ildss
{
    public static class Settings
    {
        private static IList<string> IgnoredExtensions = new List<string> { ".tmp", ".TMP", ".gz", ".ini", "~$", ".ildss" };
        //private static string WorkingDir = @"E:\Documents\GitHub\docstore\TestDir"; 
        private static string WorkingDir = @"C:\Users\Max\Documents\GitHub\docstore\TestDir";
        //private static string StorageDir = @"E:\Documents\GitHub\docstore\StorageDir";
        private static string StorageDir = @"C:\Users\Max\Documents\GitHub\docstore\StorageDir";
        private static bool FirstRun = true;
        private static int IndexInterval = 3000; // default to 1 hour
        private static int IncrementalBackupInterval = 1;
        public static string BucketName = "maxrevittildss";
        public static long TargetDiskUtilisation = 1024; // default to 1MB (more like 30 gig)
        public static TimeSpan TargetDocumentMaxAge = TimeSpan.FromSeconds(120); // default to 120 seconds (more like 30 days)
  
        // enumerated variables
        public enum DocStatus { Indexed, Current, Archived, Permanent };
        public enum EventType { Read, Write, Create, Rename };
        public enum BackupType { Incremental, Full };


        public static IList<string> getIgnoredExtensions()
        {
            return IgnoredExtensions;
        }
        public static void setIgnoredExtensions(IList<string> IE)
        {
            IgnoredExtensions = IE;
            UpdateSettings();
        }

        public static string getWorkingDir()
        {
            return WorkingDir;
        }
        public static void setWorkingDir(string WD)
        {
            WorkingDir = WD;
            UpdateSettings();
        }

        public static string getStorageDir()
        {
            return StorageDir;
        }
        public static void setStorageDir(string SD)
        {
            StorageDir = SD;
            UpdateSettings();
        }

        public static bool getFirstRun()
        {
            return FirstRun;
        }
        public static void setFirstRun(bool FR)
        {
            FirstRun = FR;
            UpdateSettings();
        }

        public static int getIndexInterval()
        {
            return IndexInterval;
        }
        public static void setIndexInterval(int II)
        {
            IndexInterval = II;
            UpdateSettings();
        }

        public static int getBackupInterval()
        {
            return IncrementalBackupInterval;
        }
        public static void setBackupInterval(int BI)
        {
            IncrementalBackupInterval = BI;
            UpdateSettings();
        }


        public static void InitSettings()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();

            if (fic.StoredSettings.Count() == 1)
            {
                LoadSettings();
            }
            else
            {
                UpdateSettings();
            }
        }

        public static void LoadSettings()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var dbSettings = fic.StoredSettings.FirstOrDefault();

            IgnoredExtensions = dbSettings.IgnoredExtensions.Split(',').ToList();
            WorkingDir = dbSettings.WorkingDir;
            StorageDir = dbSettings.StorageDir;
            FirstRun = dbSettings.FirstRun;
            IndexInterval = dbSettings.IndexInterval;
        }

        public static void UpdateSettings()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            StoredSettings dbSettings;

            if (fic.StoredSettings.Any())
            {
                dbSettings = fic.StoredSettings.FirstOrDefault();
            }
            else
            {
                dbSettings = new StoredSettings();
                fic.StoredSettings.Add(dbSettings);
            }

            dbSettings.IgnoredExtensions = String.Join(",", IgnoredExtensions);
            dbSettings.WorkingDir = WorkingDir;
            dbSettings.StorageDir = StorageDir;
            dbSettings.FirstRun = FirstRun;
            dbSettings.IndexInterval = IndexInterval;

            fic.SaveChanges();

        }

    }
}
