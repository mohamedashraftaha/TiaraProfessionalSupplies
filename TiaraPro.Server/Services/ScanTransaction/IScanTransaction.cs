using TiaraPro.Server.Models;

namespace TiaraPro.Server.Services.ScanTransaction;

public interface IScanTransaction
{
    public Task<bool> AddUserTransaction(string email, string guid, string s3Url, string status);

    public Task<List<Transactions?>?> GetUserTransactions(int id);

} 