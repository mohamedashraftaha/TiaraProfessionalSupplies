using TiaraPro.Server.Services.ScanTransaction;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using TiaraPro.Server.Services.DentalMeshAI;

namespace TiaraPro.Server.Services.BackgroundWorker
{
    public class BackgroundListenerService : BackgroundService
    {
        private readonly ILogger<BackgroundListenerService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BackgroundListenerService(ILogger<BackgroundListenerService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int delay = 300000;
            //int delay = 5000;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Background listener is running...");

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dentalMesh = scope.ServiceProvider.GetRequiredService<IDentalMeshAI>();
                    await dentalMesh.ProcessPendingTransactionsAsync();

                }

                await Task.Delay(delay, stoppingToken);  
            }

            _logger.LogInformation("Background listener has stopped.");
        }
    }
} 