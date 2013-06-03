using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudInterface;
using Ildss.Index;

namespace Ildss.Storage
{
    class CloudStorage : IStorage
    {
        public CloudStorage()
        {
            // create bucket if it doesn't exist & set policies
            // read bucket name from settings
            var manager = KernelFactory.Instance.Get<ICloudManager>();
            manager.CreateBucketIfNotExists(Settings.BucketName);
        }

        public async void StoreIncrAsync()
        {
            // get list of files which need backing up
            // upload files

            var reader = KernelFactory.Instance.Get<IReader>();
            var documents = reader.GetFilesForIncrementalBackup();
            await Upload.UploadAsync(documents, new Progress<int>(), Settings.BucketName);

            // update database using StatusChanger - set status to current
            var versionManager = KernelFactory.Instance.Get<IVersionManager>();
            versionManager.AddVersion(Settings.DocStatus.Current, documents);
        }

        public async void RemoveUnusedDocumentsAsync()
        {
            // get list of unused files
            // delete from local drive
            var reader = KernelFactory.Instance.Get<IReader>();
            await Remove.RemoveLocalAsync(reader.GetUnusedFilesForLocalDeletion());

            // update database using reader - set status to archived
        }
    }
}
