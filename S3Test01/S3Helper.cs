using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;

namespace S3Test01
{
    public class S3Helper
    {
        public static bool UploadFile(IAmazonS3 client, string bucketName, string sourceFilename, string objectKey)
        {
            TransferUtility utility = new TransferUtility(client);

            try
            {
                //utility.Upload(sourceFilename, bucketName, objectKey);
                using (FileStream fileToUpload =
                        new FileStream(sourceFilename, FileMode.Open, FileAccess.Read))
                {
                    utility.Upload(fileToUpload, bucketName, objectKey);
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine(
                        "For service sign up go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(
                        "Error occurred. Message:'{0}' when writing an object"
                        , amazonS3Exception.Message);
                }
            }


            return true; //indicate that the file was sent  
        }

        public static void DownloadFile(IAmazonS3 client, string bucketName, string objectKey, string localFilePath)
        {
            TransferUtility transferUtility = new TransferUtility(client);
            transferUtility.Download(localFilePath, bucketName, objectKey);
        }

    }
}
