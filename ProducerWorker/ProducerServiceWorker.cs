using System;
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
            if (producerWorkerConfigProvider.UseExchange())
                Console.WriteLine($"Publishing on exchange '{producerWorkerConfigProvider.GetExchangeName()}' using routing '{producerWorkerConfigProvider.GetRoutingKey()}. Exchange type: {producerWorkerConfigProvider.GetExchangeType()}'");
            else
                Console.WriteLine($"Publishing on queue '{producerWorkerConfigProvider.GetQueueName()}'");

            var i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (producerWorkerConfigProvider.UseExchange())
                    await eventBusDispatcher.ExchangePublishAsync(
                        $"[Worker {producerWorkerConfigProvider.GetWorkerName()}] Message {++i}",
                        producerWorkerConfigProvider.GetExchangeName(),
                        producerWorkerConfigProvider.GetRoutingKey(),
                        producerWorkerConfigProvider.GetExchangeType());
                else
                    await eventBusDispatcher.QueuePublishAsync(
                        $"[Worker {producerWorkerConfigProvider.GetWorkerName()}] Message {++i}",
                        producerWorkerConfigProvider.GetQueueName());

                await Task.Delay(producerWorkerConfigProvider.GetProductionInterval(), stoppingToken);
            }
        }
    }
}
