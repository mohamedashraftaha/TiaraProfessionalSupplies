namespace TiaraPro.Server.Services.AwsS3;

public interface IAWSS3Service
{
    Task<(string publicUrl, string signedUrl)> UploadFileAsync(IFormFile file, string guid);

    Task<string> UploadImage(IFormFile file);
} 