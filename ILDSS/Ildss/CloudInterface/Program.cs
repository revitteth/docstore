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
    public static class Program
    {
        static string existingBucketName = "ildss";

        // LONG RUNNING TASK FOR UPLOADS?

        public static int UploadFile(string file)
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

                uploadRequest.UploadProgressEvent +=
                    new EventHandler<UploadProgressArgs>
                        (uploadRequest_UploadPartProgressEvent);

                fileTransferUtility.Upload(uploadRequest);

                Console.WriteLine("Upload completed");

                return 1000;
            }

            catch (AmazonS3Exception e)
            {
                Console.WriteLine(e.Message + e.InnerException);
            }
            return 100;
        }

        static void uploadRequest_UploadPartProgressEvent(
            object sender, UploadProgressArgs e)
        {
            // Process event.
            Console.WriteLine("{0}/{1} " +  e.TransferredBytes + " " + e.TotalBytes);
        }

        public static Task<int> UploadFileAsync(string file)
        {
            return Task<int>.Run(() =>
                UploadFile(file));
        }
    }
}