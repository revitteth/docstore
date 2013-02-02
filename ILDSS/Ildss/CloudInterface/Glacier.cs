using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Glacier;
using Amazon.Glacier.Model;
using Amazon.Glacier.Transfer;

namespace CloudInterface
{
    class Glacier
    {
        AmazonGlacierClient _gc = new AmazonGlacierClient();

        public Boolean uploadToArchive(string localPath, string vaultName)
        {
            vaultName = "examplevault";
            string archiveToUpload = @"c:\folder\exampleArchive.zip";

            try
            {
                var manager = new ArchiveTransferManager(Amazon.RegionEndpoint.EUWest1);
                string archiveId = manager.Upload(vaultName, "Tax 2012 documents", archiveToUpload).ArchiveId;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("it didn't work");
                return false;
            }
        }
    }
}