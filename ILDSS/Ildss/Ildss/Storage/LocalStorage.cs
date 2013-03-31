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

        public void Store()
        {
            var fic = KernelFactory.Instance.Get<IFileIndexContext>();
            var toStore = fic.Documents.Where(i => i.status == "Indexed").ToList();

            cleanDir(tmp);

            try
            {
                if (toStore.Count() > 0)
                {
                    foreach (var doc in toStore)
                    {
                        // move to tmp
                        var d = new FileInfo(doc.DocPaths.First().path);
                        d.CopyTo(tmp + doc.DocumentHash);

                    }

                    //zip up the tmp
                    var zipFile = new FileInfo(ildssDir + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".zip");
                    ZipFile.CreateFromDirectory(tmp, zipFile.FullName);

                    // move it to storage
                    zipFile.MoveTo(Path.Combine(storageDir, zipFile.Name));

                    Logger.write("Created local storage backup \'" + zipFile.Name + "\' containing " + toStore.Count() + " files");
                }
                else
                {
                    Logger.write("No files to be backed up");
                }
            }
            catch (IOException e)
            {
                Logger.write(e.Message);
                return;
            }

            // set all database docs to be "Current"
            foreach (var doc in toStore)
            {
                doc.status = "Current";
            }
            fic.SaveChanges();

            cleanDir(tmp);

        }

        public void Retrieve()
        {

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
