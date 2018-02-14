using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace S3Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                await MainAsync(args);
            }).GetAwaiter().GetResult();

        }
        static async Task<int> MainAsync(string[] args)
        {
         
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var key = configuration["s3key"];
            var secret = configuration["s3secret"];
            var url = configuration["s3url"];
            var bucket = configuration["s3bucket"];
            var testfile = configuration["testfile"];
            var objectkey = configuration["objectkey"];


            Console.WriteLine($"Url: {url}");
            Console.WriteLine($"Bucket: {bucket}");
            Console.WriteLine($"Testfile: {testfile}");
            Console.WriteLine($"Object Key: {objectkey}");


            Console.WriteLine("Uploading Testfile...");
            try
            {
                AWSCredentials credentials = new BasicAWSCredentials(key, secret);
                AmazonS3Config config = new AmazonS3Config();
                config.ServiceURL = url;

                AmazonS3Client client = new AmazonS3Client(credentials, config);


                var result = UploadFile(client, bucket, testfile, objectkey);

                if(result == true)
                {
                    Console.WriteLine("File uploaded!");
                }
                else
                {
                    Console.WriteLine("File NOT uploaded!");
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
                    "To sign up for service, go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(
                     "Error occurred. Message:'{0}' when listing objects",
                     amazonS3Exception.Message);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Ausnahme: " + ex.Message);
                Console.WriteLine("");
            }


            Console.WriteLine("Press any key...");

            Console.ReadKey();

//            string key = "TX2B7702UYZPVPM0GJ81", secret = "6UDYFFMgs2NQxurZEHlSyUvZ//QCOGVNcVMH6FXk";

            

            
                        

            

//            IAmazonS3 client = new AmazonS3Client(key, secret, config);


            //bool res = client.DoesS3BucketExistAsync(bucket).Result;
//            client.Config.Validate();
//          UploadFile(client, bucket, file, objectKey);
            /*
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = "sam-test",
                Key = objectKey,
                FilePath = file
            };
            PutObjectResponse response2 = client.PutObjectAsync(request).Result;
            */

            //IAmazonS3 client = new AmazonS3Client(tempCredentials, config);



/*                UploadFile(client, bucket, file, objectKey);

                var nf = "Test2File.docx";
                DownloadFile(client, bucket, objectKey, nf);

    */
            return 0;
        }


        public static void ListObjects(IAmazonS3 client, string bucketName)
        {
            Console.WriteLine("Listing objects stored in a bucket");
            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    MaxKeys = 2
                };

                    var response = client.ListObjectsV2Async(request).Result;

                    // Process response.
                    foreach (S3Object entry in response.S3Objects)
                    {
                        Console.WriteLine("key = {0} size = {1}",
                            entry.Key, entry.Size);
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
                    "To sign up for service, go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(
                     "Error occurred. Message:'{0}' when listing objects",
                     amazonS3Exception.Message);
                }
            }
        }



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
                return false;
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
