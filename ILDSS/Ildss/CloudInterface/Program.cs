using System;
using System.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Specialized;
using Amazon.S3.Transfer;
using System.IO;


namespace CloudInterface
{
    public static class Program
    {
        static string existingBucketName = "ildss";
        static string filePath = @"C:\Users\Max\Documents\GitHub\docstore\TestDir\2012-03-17 13.01.03.jpg";

        public static void Upload()
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
                    .WithFilePath(filePath);

                uploadRequest.UploadProgressEvent +=
                    new EventHandler<UploadProgressArgs>
                        (uploadRequest_UploadPartProgressEvent);

                fileTransferUtility.Upload(uploadRequest);
                Console.WriteLine("Upload completed");
            }

            catch (AmazonS3Exception e)
            {
                Console.WriteLine(e.Message + e.InnerException);
            }
        }

        static void uploadRequest_UploadPartProgressEvent(
            object sender, UploadProgressArgs e)
        {
            // Process event.
            Console.WriteLine("{0}/{1} " +  e.TransferredBytes + " " + e.TotalBytes);
        }
    }
}