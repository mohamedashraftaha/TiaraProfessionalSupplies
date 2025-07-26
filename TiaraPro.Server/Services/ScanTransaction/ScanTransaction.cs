using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;

namespace TiaraPro.Server.Services.ScanTransaction;

public class ScanTransaction : IScanTransaction
{
    private readonly ILogger<ScanTransaction> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public ScanTransaction(ILogger<ScanTransaction> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> AddUserTransaction(string email, string guid, string s3Url, string status)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(status))
            {
                _logger.LogWarning("Invalid parameters provided for adding user transaction.");
                return false;
            }
            #region Add User Transaction
            await _unitOfWork.BeginTransactionAsync();
            var txnGuid = Guid.TryParse(guid, out var GUID) ? GUID : (Guid?)null;
            if (txnGuid == null)
            {
                _logger.LogWarning($"Invalid GUID format: {guid}");
                return false;
            }
            var transaction = new Transactions
            {
                TransactionGuid = txnGuid.Value,
                Status = status,
                User = await _unitOfWork.Users.GetByEmailAsync(email),
                S3Url = s3Url,
            };

            if (transaction.User == null)
            {
                _logger.LogWarning($"User with email {email} not found.");
                return false;
            }
            transaction.UserId = transaction.User.Id;

            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.CompleteAsync();


            #endregion
            _logger.LogInformation($"Transaction added for user {email} with GUID {guid} and status {status}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user transaction.");
            await _unitOfWork.RollbackAsync();
            return false;
        }
    }

    public Task<List<Transactions?>?> GetUserTransactions(int id)
    {
        try
        {
            return _unitOfWork.Transactions.GetTransactionsByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user transactions for user ID {UserId}", id);
            return Task.FromResult<List<Transactions?>?>(null);
        }
    }

    } 