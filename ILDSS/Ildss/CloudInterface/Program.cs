using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace CloudInterface
{
    class CloudInterface
    {
        public CloudInterface()
        {
            AmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(RegionEndpoint.EUWest1);

        }
    }
}