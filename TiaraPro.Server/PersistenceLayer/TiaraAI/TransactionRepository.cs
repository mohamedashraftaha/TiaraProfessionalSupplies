using TiaraPro.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace TiaraPro.Server.PersistenceLayer.TiaraAI;

public class TransactionRepository : ITransactionRepository
{
    private readonly TiaraDbContext _context;

    public TransactionRepository(TiaraDbContext context)
    {
        _context = context;
    }

    public async Task<Transactions> GetByIdAsync(int id)
    {
        return await _context.Transactions.FindAsync(id);
    }

    public async Task<Transactions> GetByTransactionGuidAsync(string transactionGuid)
    {
        return await _context.Transactions
                             .FirstOrDefaultAsync(t => t.TransactionGuid.ToString() == transactionGuid);
    }

    public async Task<IEnumerable<Transactions>> GetAllAsync()
    {
        return await _context.Transactions.ToListAsync();
    }

    public async Task<List<Transactions?>?> GetTransactionsByIdAsync(int id)
    {
        try
        {

            return await _context.Transactions
                                 .Where(t => t.UserId == id)
                                 .ToListAsync();

        }
        catch (Exception ex)
        {
            // Log the exception (consider using a logging framework)
            Console.WriteLine($"Error retrieving transactions for user ID {id}: {ex.Message}");
            return new List<Transactions?>();
        }
    }

    public async Task AddAsync(Transactions transaction)
    {
        await _context.Transactions.AddAsync(transaction);
    }

    public void Update(Transactions transaction)
    {
        _context.Transactions.Update(transaction);
    }

    public void Delete(Transactions transaction)
    {
        _context.Transactions.Remove(transaction);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

}