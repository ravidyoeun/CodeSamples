using Amazon.S3.Model;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Requests.Uploads;
using System;
using System.Data.SqlClient;

namespace Sabio.Web.Services.S3Service
{
    public class S3Handler : BaseService
    {
        // Declare Variables
        private static string S3Bucket = ConfigService.GetAWS_S3Bucket();
        // US West Oregon Service url.
        private static string ServiceUrl = ConfigService.GetAWS_ServiceUrl();
        private static string AccessKey = ConfigService.GetAWS_AccessKey();
        private static string SecretKey = ConfigService.GetAWS_SecretKey();


        //- Instantiate S3Client
        Amazon.S3.AmazonS3Client S3Client = null;

        public S3Handler()
        {
            Amazon.S3.AmazonS3Config s3Config = new Amazon.S3.AmazonS3Config();
            // Build Url.
            s3Config.ServiceURL = ServiceUrl;

            //Amazon.S3.AmazonS3Client S3Client = null;
            this.S3Client = new Amazon.S3.AmazonS3Client(AccessKey, SecretKey, s3Config);
        }

        public string UploadFile(S3Request model)
        {
            //- Build Put Object
            var s3PutRequest = new Amazon.S3.Model.PutObjectRequest
            {
                FilePath = model.FilePath,
                BucketName = S3Bucket,
                CannedACL = Amazon.S3.S3CannedACL.PublicRead
            };

            if (!string.IsNullOrWhiteSpace(model.NewFileName))
            {
                s3PutRequest.Key = model.NewFileName;
            }

            try
            {
                Amazon.S3.Model.PutObjectResponse s3PutResponse = this.S3Client.PutObject(s3PutRequest);

                if (model.DeleteLocalFileOnSuccess)
                {
                    if (System.IO.File.Exists(model.FilePath))
                    {
                        System.IO.File.Delete(model.FilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            string url = ServiceUrl + "/" + S3Bucket + "/" + model.NewFileName;
            return url;
        }



        // TO UPLOAD STREAM TO AWS
        public string UploadStream(S3Request model)
        {
            //- Build Put Object
            PutObjectRequest s3PutRequest = new PutObjectRequest
            {
                BucketName = S3Bucket,
                CannedACL = Amazon.S3.S3CannedACL.PublicRead,
                Key = model.NewFileName

        };

            try
            {
                using (model.FileStream) {

                    s3PutRequest.InputStream = model.FileStream;
                    PutObjectResponse s3PutResponse = this.S3Client.PutObject(s3PutRequest);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            string url = ServiceUrl + "/" + S3Bucket + "/" + model.NewFileName;
            return url;
        }



    }
}



