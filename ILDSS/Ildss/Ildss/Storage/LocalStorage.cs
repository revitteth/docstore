using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ildss.Models;

namespace Ildss.Storage
{
    class LocalStorage : IStorage
    {
        private string ildssDir = @"C:\ildss\";
        private string tmp = @"C:\ildss\tmp\";
        private string storageDir = Properties.Settings.Default.storageDir;

        public LocalStorage()
        {
            if (!Directory.Exists(ildssDir))
            {
                Directory.CreateDirectory(ildssDir);
            }
            if (!Directory.Exists(tmp))
            {
                Directory.CreateDirectory(tmp);
            }
        }

        public void StoreIncr()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var toStore = fic.Documents.Where(i => i.Status == "Indexed").ToList();
            string backupname = null;
            DateTime backuptime = DateTime.Now;

            cleanDir(tmp);

            try
            {
                if (toStore.Count() > 0)
                {
                    foreach (var doc in toStore)
                    {
                        // move to tmp
                        var d = new FileInfo(doc.DocPaths.First().Path);
                        d.CopyTo(tmp + doc.DocumentHash);

                    }

                    //zip up the tmp
                    var zipFile = new FileInfo(ildssDir + backuptime.ToString("ddMMyyyy-hhmmss") + "-incr.zip");
                    ZipFile.CreateFromDirectory(tmp, zipFile.FullName);

                    // move it to storage
                    zipFile.MoveTo(Path.Combine(storageDir, zipFile.Name));
                    backupname = zipFile.Name;

                    Logger.write("Success Created Incremental Backup \'" + zipFile.Name + "\' containing " + toStore.Count() + " files");
                }
                else
                {
                    Logger.write("No files to be backed up");
                }
            }
            catch (Exception e)
            {
                Logger.write("Error " + e.Message);
                return;
            }

            var bup = new Backup();

            // set all database docs to be "Current" and add documents to backup table entry
            foreach (var doc in toStore)
            {
                doc.Status = "Current";
                bup.Documents.Add(doc);
                doc.Backups.Add(bup);
            }

            bup.Name = backupname;
            bup.Time = backuptime;
            bup.Type = "Incr";

            fic.SaveChanges();

            cleanDir(tmp);

        }

        public void StoreFull()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            string backupname = null;
            DateTime backuptime = DateTime.Now;

            cleanDir(tmp);

            try
            {
                foreach (var doc in fic.Documents)
                {
                    // move to tmp
                    var d = new FileInfo(doc.DocPaths.First().Path);
                    d.CopyTo(tmp + doc.DocumentHash);

                }

                //zip up the tmp
                var zipFile = new FileInfo(ildssDir + DateTime.Now.ToString("ddMMyyyy-hhmmss") + "-full.zip");
                ZipFile.CreateFromDirectory(tmp, zipFile.FullName);

                // move it to storage
                zipFile.MoveTo(Path.Combine(storageDir, zipFile.Name));

                Logger.write("Success Created Full Backup \'" + zipFile.Name + "\' containing " + fic.Documents.Count() + " files");
            }
            catch (Exception e)
            {
                Logger.write("Error " + e.Message);
                return;
            }

            var bup = new Backup();

            // set all database docs to be "Current" and add documents to backup table entry
            foreach (var doc in fic.Documents)
            {
                doc.Status = "Current";
                bup.Documents.Add(doc);
            }

            bup.Name = backupname;
            bup.Time = backuptime;
            bup.Type = "Incr";
            fic.SaveChanges();

            cleanDir(tmp);

        }

        public void RetrieveIncr()
        {
            // identify the date of the increment and pull it back

            // search the events table for the correct event? or backup table
        }

        public void RetrieveFull()
        {
            // identify the date of the full backup and pull it back
        }

        private void cleanDir(string dir)
        {
            // clean directory
            string[] paths = Directory.GetFiles(dir);
            foreach (var path in paths)
            {
                File.Delete(path);
            }
        }

    }


}
