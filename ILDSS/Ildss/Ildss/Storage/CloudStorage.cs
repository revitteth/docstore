using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudInterface;
using Ildss.Index;
using Ildss.Models;

namespace Ildss.Storage
{
    class CloudStorage : IStorage
    {
        public CloudStorage()
        {
            // create bucket if it doesn't exist & set policies
            // read bucket name from settings
            var manager = KernelFactory.Instance.Get<ICloudManager>();
            manager.CreateBucketIfNotExists(Settings.Default.S3BucketName);
        }

        public async void StoreIncrAsync()
        {
            // get list of files which need backing up
            // upload files

            var reader = KernelFactory.Instance.Get<IReader>();
            var documents = reader.GetFilesForIncrementalBackup();
            await Upload.UploadAsync(documents, new Progress<int>(), Settings.Default.S3BucketName);

            // update database using StatusChanger - set status to current
            var versionManager = KernelFactory.Instance.Get<IVersionManager>();
            versionManager.AddVersion(Enums.DocStatus.Current, documents);
        }

        public void Retrieve(string key, string dest, Document doc)
        {
            Download.DownloadFile(key, Settings.Default.S3BucketName, dest);
            var vm = KernelFactory.Instance.Get<IVersionManager>();
            vm.UpdateStatus(Enums.DocStatus.Current, doc);
        }

        public async void RemoveUnusedDocumentsAsync()
        {
            // get list of unused files
            // delete from local drive
            var reader = KernelFactory.Instance.Get<IReader>();
            var unusedDocuments = reader.GetUnusedDocumentsForLocalDeletion();

            var unusedFiles = reader.GetUnusedFilesForLocalDeletion();

            await Remove.RemoveLocalAsync(unusedFiles);

            var versionManager = KernelFactory.Instance.Get<IVersionManager>();
            versionManager.UpdateStatus(Enums.DocStatus.Archived, unusedDocuments);
            // update database using reader - set status to archived
        }
    }
}
