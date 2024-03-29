﻿using Amazon.S3;
using Amazon.S3.Model;
using Log;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudInterface
{
    public static class Download
    {
        public static void DownloadFile(string keyName, string bucketName, string dest)
        {
            NameValueCollection appConfig = ConfigurationManager.AppSettings;
            string accessKeyID = appConfig["AWSAccessKey"];
            string secretAccessKey = appConfig["AWSSecretKey"];
            
            AmazonS3 client;
            client = Amazon.AWSClientFactory.CreateAmazonS3Client(
            accessKeyID, secretAccessKey);

            GetObjectRequest request = new GetObjectRequest().WithBucketName(bucketName).WithKey(keyName);

            using (GetObjectResponse response = client.GetObject(request))
            {
                if (File.Exists(dest))
                {
                    Logger.Write("Overwriting " + dest);
                }

                response.WriteResponseStreamToFile(dest);
            }
        }

        public static void DownloadFiles(List<Tuple<string, string, string>> files)
        {
            foreach(var file in files)
            {
                DownloadFile(file.Item1, file.Item2, file.Item3);
            }
        }
    }
}
