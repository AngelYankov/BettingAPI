using BettingAPI.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BettingAPI.Services
{
    public class BackgroundDataUploader : IHostedService, IDisposable
    {
        private readonly IBettingService bettingService;
        private Timer timer;

        public BackgroundDataUploader(IServiceProvider serviceProvider)
        {
            this.bettingService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IBettingService>();
        }

        public void Dispose()
        {
            timer?.Dispose(); 
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(t =>
            {
                this.bettingService.SaveData();
            }
            , null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
