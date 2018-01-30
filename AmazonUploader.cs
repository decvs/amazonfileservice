using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AmazonFileService
{
    public class AmazonUploader
    {
        public AmazonProfile Profile { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
        public string BucketName { get; set; }

        public bool UploadFile(string bucketName, string sourceFilename, string objectKey)
        {
            IAmazonS3 client = new AmazonS3Client(this.RegionEndpoint);
            TransferUtility utility = new TransferUtility(client);

            using (FileStream fileToUpload =
                    new FileStream(sourceFilename, FileMode.Open, FileAccess.Read))
            {
                utility.Upload(fileToUpload,
                                           bucketName, objectKey);
            }

            return true; //indicate that the file was sent  
        }

        public void DownloadFile(string bucketName, string objectKey, string localFilePath)
        {
            IAmazonS3 client = new AmazonS3Client(this.RegionEndpoint);
            TransferUtility transferUtility = new TransferUtility(client);
            transferUtility.Download(localFilePath, bucketName, objectKey);
        }
    }
}