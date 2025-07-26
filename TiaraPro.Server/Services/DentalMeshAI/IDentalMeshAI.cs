namespace TiaraPro.Server.Services.DentalMeshAI
{
    public interface IDentalMeshAI
    {
        Task<string> UploadFileAsync(string email, IFormFile file);
        Task ProcessPendingTransactionsAsync();
    }
}
