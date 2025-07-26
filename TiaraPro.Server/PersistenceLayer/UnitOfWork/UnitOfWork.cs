using Microsoft.EntityFrameworkCore.Storage;
using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer.CategoriesRepository;
using TiaraPro.Server.PersistenceLayer.Notifications;
using TiaraPro.Server.PersistenceLayer.OrderItems;
using TiaraPro.Server.PersistenceLayer.OrdersRepository;
using TiaraPro.Server.PersistenceLayer.Payments;
using TiaraPro.Server.PersistenceLayer.ProductsRepository;
using TiaraPro.Server.PersistenceLayer.ProductVariants;
using TiaraPro.Server.PersistenceLayer.PromoCodeUsage;
using TiaraPro.Server.PersistenceLayer.TiaraAI;
using TiaraPro.Server.PersistenceLayer.TiaraDentalTraining;
using TiaraPro.Server.PersistenceLayer.UserRepositories;

namespace TiaraPro.Server.PersistenceLayer.UnitOfWork;

public class UnitOfWork :IUnitOfWork
{
    private readonly TiaraDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly ILoggerFactory _loggerFactory;
    public UnitOfWork(TiaraDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _loggerFactory = loggerFactory;
    }
    public IUserRepository Users => new UserRepository(_context);
    public IProductRepository Products => new ProductRepository(_context);
    public ICategoryRepository Categories => new CategoryRepository(_context);
    public IOrderRepository Orders => new OrderRepository(_context, _loggerFactory.CreateLogger<OrderRepository>());
    public IPaymentsRepository Payments => new PaymentsRepository(_context, _loggerFactory.CreateLogger<PaymentsRepository>());

    public IOrderItemsRepository OrderItems => new OrderItemsRepository(_context);

    public IProductVariantsRepository ProductVariants => new ProductVariantsRepository(_context);

    public INotificationRepository Notifications => new NotificationRepository(_context);

    public ITiaraAISubscriptionRepository TiaraAISubscriptions => new TiaraAISubscriptionRepository(_context);

    public IUserSubscriptionRepository UserSubscriptions => new UserSubscriptionRepository(_context);
    
    public ITransactionRepository Transactions => new TransactionRepository(_context);

    public IDentalTrainingRepository DentalTraining => new DentalTrainingRepository(_context);


    public IPromoCodeUsageRepository PromoCodeUsages => new PromoCodeUsageRepository(_context);
    public async Task<int> CompleteAsync()
    {
        try
        {
            int result = await _context.SaveChangesAsync();
            await _transaction!.CommitAsync();  // Commit the transaction
            return result;
        }
        catch
        {
            await RollbackAsync();  // Rollback in case of any error
            throw;
        }
    }
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
