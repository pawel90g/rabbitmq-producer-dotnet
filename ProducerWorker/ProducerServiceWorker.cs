using System.Threading;
using System.Threading.Tasks;
using EventsDispatcher.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProducerWorker.Services.Interfaces;

namespace ProducerWorker
{
    public class ProducerServiceWorker : BackgroundService
    {
        private readonly ILogger<ProducerServiceWorker> _logger;
        private readonly IEventBusDispatcher eventBusDispatcher;
        private readonly IProducerWorkerConfigProvider producerWorkerConfigProvider;

        public ProducerServiceWorker(
            ILogger<ProducerServiceWorker> logger,
            IEventBusDispatcher eventBusDispatcher, 
            IProducerWorkerConfigProvider producerWorkerConfigProvider)
        {
            _logger = logger;
            this.eventBusDispatcher = eventBusDispatcher;
            this.producerWorkerConfigProvider = producerWorkerConfigProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                await eventBusDispatcher.PublishAsync($"[Worker {producerWorkerConfigProvider.GetWorkerName()}] Message {++i}", producerWorkerConfigProvider.GetQueueName());
                await Task.Delay(producerWorkerConfigProvider.GetProductionInterval(), stoppingToken);
            }
        }
    }
}
