using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using TiaraPro.Server.Authentication;
using TiaraPro.Server.Models;
using TiaraPro.Server.PaymentProvider;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;
using TiaraPro.Server.Services.DentalMeshAI;
using TiaraPro.Server.Services.AwsS3;
using TiaraPro.Server.Services.ScanTransaction;
using TiaraPro.Server.Services.EmailService;
using TiaraPro.Server.Services.BackgroundWorker;
using Microsoft.AspNetCore.Http.Features;
using TiaraPro.Server.PersistenceLayer.PromoCodeUsage;
using TiaraPro.Server.PersistenceLayer.Notifications;
using TiaraPro.Server.PersistenceLayer.OrderItems;
using TiaraPro.Server.PersistenceLayer.ProductVariants;
using TiaraPro.Server.PersistenceLayer.Payments;
using TiaraPro.Server.PersistenceLayer.TiaraAI;
using TiaraPro.Server.Services.Notifications;
using TiaraPro.Server.Services.Payments;
using TiaraPro.Server.Services.TiaraAI;
using TiaraPro.Server.Services.PromoCodes;
using TiaraPro.Server.PersistenceLayer.UserRepositories;
using TiaraPro.Server.Services.UsersService;
using TiaraPro.Server.Services.OrdersService;
using TiaraPro.Server.Services.CategoriesService;
using TiaraPro.Server.Services.ProductsService;
using TiaraPro.Server.PersistenceLayer.CategoriesRepository;
using TiaraPro.Server.PersistenceLayer.ProductsRepository;
using TiaraPro.Server.PersistenceLayer.OrdersRepository;
using TiaraPro.Server.PersistenceLayer.TiaraDentalTraining;
using TiaraPro.Server.Services.TiaraDentalTraining;
using DentalTraining = TiaraPro.Server.Services.TiaraDentalTraining.DentalTraining;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Increase max body size to 100MB
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100MB
});

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Register DbContext
builder.Services.AddDbContext<TiaraDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


var configuration = builder.Configuration;
builder.Services.AddHttpClient();
builder.Services.Configure<PaymobOptions>(configuration.GetSection("Paymob"));
builder.Services.AddScoped<IPaymobService, PaymobService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPaymentsService, PaymentsService>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();
builder.Services.AddScoped<ITiaraAISubscriptionService, TiaraAISubscriptionService>();
builder.Services.AddScoped<IDentalTraining, DentalTraining>();

builder.Services.AddScoped<IJWTToken, JWTToken>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<IOrderItemsRepository, OrderItemsRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IProductVariantsRepository, ProductVariantsRepository>();
builder.Services.AddScoped<ITiaraAISubscriptionRepository, TiaraAISubscriptionRepository>();
builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
builder.Services.AddScoped<IPromoCodeService, PromoCodeService>();
builder.Services.AddScoped<IPromoCodeUsageRepository, PromoCodeUsageRepository>();
// Register AI-related services
builder.Services.AddScoped<IDentalMeshAI, DentalMeshAI>();
builder.Services.AddScoped<IAWSS3Service, AWSS3Service>();
builder.Services.AddScoped<IScanTransaction, ScanTransaction>();
builder.Services.AddScoped<IEmailHandler, EmailHandler>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IDentalTrainingRepository, DentalTrainingRepository>();

// Register background service for processing AI scans
builder.Services.AddHostedService<BackgroundListenerService>();


var JwtConfiguration = builder.Configuration.GetSection("Jwt");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:3000", "https://tiarapro.com", "https://981c640cbab0.ngrok-free.app")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtConfiguration["Issuer"], 
        ValidAudience = JwtConfiguration["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfiguration["SecretKey"]!))
    };
});

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

// Seed TiaraAI Subscriptions
using (var scope = app.Services.CreateScope())
{
    var subscriptionService = scope.ServiceProvider.GetRequiredService<ITiaraAISubscriptionService>();
    await subscriptionService.SeedDefaultSubscriptionsAsync();
}

await app.RunAsync();
