using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BettingAPI.Services
{
    public class BackgroundDataUploader : IHostedService, IDisposable
    {
        private readonly IBettingServiceNew bettingService;
        private Timer timer;

        public BackgroundDataUploader(IServiceProvider serviceProvider)
        {
            this.bettingService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IBettingServiceNew>();
        }

        public void Dispose()
        {
            timer?.Dispose(); 
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(t => this.bettingService.Save(), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
