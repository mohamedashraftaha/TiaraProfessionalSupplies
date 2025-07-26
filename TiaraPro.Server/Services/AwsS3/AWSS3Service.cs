using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon.S3.Util;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;
using System.Text;

namespace TiaraPro.Server.Services.AwsS3
{
    public class AWSS3Service : IAWSS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly IConfiguration _configuration;

        public AWSS3Service(IConfiguration configuration)
        {
            _configuration = configuration;
            var awsOptions = _configuration.GetAWSOptions();
            var credentials = new BasicAWSCredentials(
                _configuration["AWS:AccessKey"],
                _configuration["AWS:SecretKey"]
            );
            _s3Client = new AmazonS3Client(credentials, awsOptions.Region);
            _bucketName = _configuration["AWS:BucketName"];
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File cannot be null or empty", nameof(file));
                }
                var key = $"images/{file.FileName}";
                using var stream = file.OpenReadStream();
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = file.ContentType
                };
                await _s3Client.PutObjectAsync(putRequest);
                return $"https://{_bucketName}.s3.amazonaws.com/{key}";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }

        }

        public async Task<(string publicUrl, string signedUrl)> UploadFileAsync(IFormFile file, string guid)
        {
            var publicUrl =string.Empty;
            var signedUrl =string.Empty;

            try
            {
                if (file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    var key = $"uploads/{guid}_{file.FileName}";

                    using var stream = file.OpenReadStream();

                    var putRequest = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        InputStream = stream,
                        ContentType = "application/zip",
                    };

                    await _s3Client.PutObjectAsync(putRequest);

                    publicUrl= $"https://{_bucketName}.s3.amazonaws.com/{key}";

                    var signedUrlRequest = new GetPreSignedUrlRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        Expires = DateTime.UtcNow.AddMinutes(60)
                    };
                    signedUrl =_s3Client.GetPreSignedURL(signedUrlRequest);
                }
                else if (file.FileName.EndsWith(".dcm", StringComparison.OrdinalIgnoreCase))
                {
                    var key = $"uploads/{guid}_{file.FileName}";

                    using var stream = file.OpenReadStream();

                    var putRequest = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        InputStream = stream,
                        ContentType = "application/dicom", // DICOM content type
                    };

                    await _s3Client.PutObjectAsync(putRequest);

                    publicUrl = $"https://{_bucketName}.s3.amazonaws.com/{key}";

                    var signedUrlRequest = new GetPreSignedUrlRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        Expires = DateTime.UtcNow.AddMinutes(60)
                    };
                    signedUrl = _s3Client.GetPreSignedURL(signedUrlRequest);
                }
                else
                {
                    throw new InvalidOperationException("Invalid file type. Please upload a DICOM file or a ZIP archive containing DICOM files.");
                }

                return (publicUrl, signedUrl);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error uploading file to AWS S3", ex);
            }
        }
    }
} 