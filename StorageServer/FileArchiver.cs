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

namespace StorageServer
{

    public class FileArchiver
    {
        
        AmazonS3Client s3Client = new AmazonS3Client();
        
        public FileArchiver()
        {
  
        }

        public getFileByKey(string fileBucket, string fileKey)
        {
            GetObjectRequest request = new GetObjectRequest {BucketName = fileBucket, Key = fileKey};

        }

        public void sendToGlacier()
        {
            //send the file to glacier - return url?!
        }

    }
}
