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
        public static IList<string> IgnoredExtensions = new List<string> { ".tmp", ".TMP", ".gz", ".ini", "~$", ".ildss" };
        public static string WorkingDir = @"E:\Documents\GitHub\docstore\TestDir"; //@"C:\Users\Max\Documents\GitHub\docstore\TestDir";
        public static string StorageDir = @"E:\Documents\GitHub\docstore\StorageDir"; //@"C:\Users\Max\Documents\GitHub\docstore\StorageDir";
        public static bool FirstRun = true;
        public static int IndexInterval = 2000;
        public static int BackupInterval = 200000;
  
        // enumerated variables
        public enum DocStatus { Indexed, Current, Archived };
        public enum EventType { Read, Write, Create, Rename };
        public enum BackupType { Incremental, Full };


        public static void InitSettings()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();

            if (fic.StoredSettings.Count() == 1)
            {
                LoadSettings();
            }
            else
            {
                FirstRun = false;
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
            BackupInterval = dbSettings.BackupInterval;
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
            dbSettings.BackupInterval = BackupInterval;

            fic.SaveChanges();

        }
    }
}
