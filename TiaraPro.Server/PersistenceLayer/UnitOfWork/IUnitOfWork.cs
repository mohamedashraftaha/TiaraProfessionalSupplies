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

public interface IUnitOfWork
{
    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task RollbackAsync();
    IUserRepository Users { get; }
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IOrderRepository Orders { get; }
    IPaymentsRepository Payments { get; }
    IOrderItemsRepository OrderItems {get;}

    INotificationRepository Notifications { get; }

    IProductVariantsRepository ProductVariants { get; }

    ITiaraAISubscriptionRepository TiaraAISubscriptions { get; }

    IUserSubscriptionRepository UserSubscriptions { get; }

    ITransactionRepository Transactions { get; }

    IPromoCodeUsageRepository PromoCodeUsages { get; }

    IDentalTrainingRepository DentalTraining { get; }
}
