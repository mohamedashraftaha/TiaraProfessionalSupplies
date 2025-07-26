using TiaraPro.Server.Models;
namespace TiaraPro.Server.PersistenceLayer.TiaraAI;

public interface ITransactionRepository
{
    Task<Transactions> GetByIdAsync(int id);
    Task<Transactions> GetByTransactionGuidAsync(string transactionGuid);

    Task<List<Transactions?>?> GetTransactionsByIdAsync(int id);

    Task<IEnumerable<Transactions>> GetAllAsync();
    Task AddAsync(Transactions transaction);
    void Update(Transactions transaction);
    void Delete(Transactions transaction);
    Task SaveAsync();
}