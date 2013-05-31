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
            await Upload.UploadAsync(reader.GetFilesForIncrementalBackup(), new Progress<int>(), Settings.BucketName);

            // update database using reader - set status to current
        }

        public async void RemoveUnusedDocumentsAsync()
        {
            // get list of unused files
            // delete from local drive
            var reader = KernelFactory.Instance.Get<IReader>();
            await Remove.RemoveLocal(reader.GetUnusedFilesForLocalDeletion());

            // update database using reader - set status to archived
        }
    }
}
