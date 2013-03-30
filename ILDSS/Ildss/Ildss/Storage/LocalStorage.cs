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
            try
            {
                var fic = KernelFactory.Instance.Get<IFileIndexContext>();
                var toStore = fic.Documents.Where(i => i.status == "Indexed").ToList();

                if (toStore.Count() > 0)
                {
                    foreach (var doc in toStore)
                    {
                        // move to tmp
                        var d = new FileInfo(doc.DocPaths.First().path);
                        d.CopyTo(tmp + doc.DocumentHash);

                    }

                    //zip up the tmp
                    var zipFile = new FileInfo(ildssDir + DateTime.Now.ToString("ddMMyyyy") + ".zip");
                    ZipFile.CreateFromDirectory(tmp, zipFile.FullName);

                    // move it to storage
                    zipFile.CopyTo(Path.Combine(storageDir, zipFile.Name));


                    // set all database docs to be "Current"
                    foreach (var doc in toStore)
                    {
                        doc.status = "Current";
                    }
                    fic.SaveChanges();


                    // clean up
                    // empty ildss directory
                    //zipFile.Delete();
                    Directory.Delete(ildssDir, true);
                    Directory.CreateDirectory(ildssDir);
                }
            }
            catch (IOException e)
            {
                Logger.write(e.Message);
            }

        }

        public void Retrieve()
        {

        }

    }


}
