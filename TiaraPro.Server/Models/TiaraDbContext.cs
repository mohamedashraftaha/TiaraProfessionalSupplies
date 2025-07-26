using Microsoft.EntityFrameworkCore;

namespace TiaraPro.Server.Models
{
    public class TiaraDbContext : DbContext
    {
        public TiaraDbContext(DbContextOptions<TiaraDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<Transactions> Transactions => Set<Transactions>();

        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        public DbSet<Notification> Notifications => Set<Notification>();

        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

        public DbSet<UserNotification> UserNotifications => Set<UserNotification>();

        public DbSet<Event> Events => Set<Event>();

        public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();

        public DbSet<DentalTraining> DentalTrainings => Set<DentalTraining>();

        public DbSet<DentalTrainingRegistration> DentalTrainingRegistrations => Set<DentalTrainingRegistration>();

        public DbSet<TiaraAISubscription> TiaraAISubscriptions => Set<TiaraAISubscription>();

        public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

        public DbSet<PromoCode> PromoCodes { get; set; }

        public DbSet<UserPromoCode> UserPromoCodes { get; set; }

        public DbSet<UserPromoCodeUsage> UserPromoCodeUsages { get; set; }

        public DbSet<DentalTrainingPackage> DentalTrainingPackages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add any additional configuration here

            // Example of a unique constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserNotification>()
                .HasIndex(un => new { un.UserId, un.NotificationId })
                .IsUnique();
            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.User)
                .WithMany()
                .HasForeignKey(un => un.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.Notification)
                .WithMany()
                .HasForeignKey(un => un.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventRegistration>()
                .HasIndex(er => new { er.UserId, er.EventId })
                .IsUnique();
            modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.User)
                .WithMany()
                .HasForeignKey(er => er.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.Event)
                .WithMany()
                .HasForeignKey(er => er.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DentalTrainingRegistration>()
                .HasIndex(dtr => new { dtr.UserId, dtr.DentalTrainingId })
                .IsUnique();
            modelBuilder.Entity<DentalTrainingRegistration>()
                .HasOne(dtr => dtr.User)
                .WithMany()
                .HasForeignKey(dtr => dtr.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DentalTrainingRegistration>()
                .HasOne(dtr => dtr.DentalTraining)
                .WithMany()
                .HasForeignKey(dtr => dtr.DentalTrainingId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserSubscription configuration
            modelBuilder.Entity<UserSubscription>()
                .HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserSubscription>()
                .HasOne(us => us.Subscription)
                .WithMany()
                .HasForeignKey(us => us.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserSubscription>()
                .HasOne(us => us.Order)
                .WithMany()
                .HasForeignKey(us => us.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
