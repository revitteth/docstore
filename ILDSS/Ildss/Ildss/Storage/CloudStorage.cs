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
            // find bucket/create if it doesn't exist (use settings to store bucket name?)
            // upload files & mark uploaded succesfully as 'current'

            var reader = KernelFactory.Instance.Get<IReader>();
            await Upload.UploadAsync(reader.GetFilesForIncrementalBackup(), new Progress<int>(), Settings.BucketName);
            // update database using reader
        }

        public async void RemoveUnusedDocumentsAsync()
        {
            var reader = KernelFactory.Instance.Get<IReader>();
            
            await Remove.RemoveLocal(reader.GetUnusedFilesForLocalDeletion());
            // update database using reader - set status to archived
        }
    }
}
