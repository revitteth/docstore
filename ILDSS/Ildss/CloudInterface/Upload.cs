using System;
using System.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Specialized;
using Amazon.S3.Transfer;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace CloudInterface
{
    public static class Upload
    {
        public static string existingBucketName;

        public static void SetBucketName(string bn)
        {
            existingBucketName = bn;
        }

        public static void UploadFile(string file)
        {
            NameValueCollection appConfig = ConfigurationManager.AppSettings;
            string accessKeyID = appConfig["AWSAccessKey"];
            string secretAccessKey = appConfig["AWSSecretKey"];

            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(accessKeyID, secretAccessKey);

                // Use TransferUtilityUploadRequest to configure options.
                // In this example we subscribe to an event.
                TransferUtilityUploadRequest uploadRequest =
                    new TransferUtilityUploadRequest()
                    .WithBucketName(existingBucketName)
                    .WithFilePath(file);

                //AmazonS3Client aws = new AmazonS3Client().PutBucketPolicy().;


                uploadRequest.UploadProgressEvent +=
                    new EventHandler<UploadProgressArgs>
                        (uploadRequest_UploadPartProgressEvent);

                fileTransferUtility.Upload(uploadRequest);

                //Console.WriteLine("Upload completed");
            }

            catch (AmazonS3Exception e)
            {
                Console.WriteLine(e.Message + e.InnerException);
            }
        }

        static void uploadRequest_UploadPartProgressEvent(object sender, UploadProgressArgs e)
        {
            // Process event.
            //Console.WriteLine("{0}/{1} " +  e.TransferredBytes + " " + e.TotalBytes);
        }

        public static Task UploadAsync(List<string> files, IProgress<int> progress)
        {
            return Task.Run(() =>
                {
                    int uploaded = 0;                   
                    foreach (var file in files)
                    {
                        UploadFile(file);
                        uploaded++;
                        progress.Report(uploaded);
                    }
                });
        }
    }
}