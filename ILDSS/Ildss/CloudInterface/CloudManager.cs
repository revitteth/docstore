using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using System.Configuration;
using System.Collections.Specialized;

namespace CloudInterface
{
    public class CloudManager : ICloudManager
    {
        public string BucketName { get; set; }
        public AmazonS3 client;

        public CloudManager()
        {
            NameValueCollection appConfig = ConfigurationManager.AppSettings;
            client = Amazon.AWSClientFactory.CreateAmazonS3Client();
        }

        public void CreateBucketIfNotExists()
        {
            var response = client.ListBuckets();

            var found = false;
            foreach (var bucket in response.Buckets)
            {
                if (bucket.BucketName == BucketName)
                {
                    found = true;
                    break;
                }
            }

            if (found == false)
            {
                try
                {
                    client.PutBucket(new PutBucketRequest().WithBucketName(BucketName));
                }
                catch (Exception ex)
                {
                    // to be handled
                }
            }
        }

        public void CreateBucketIfNotExists(string name)
        {
            BucketName = name;
            CreateBucketIfNotExists();
        }

    }
}
