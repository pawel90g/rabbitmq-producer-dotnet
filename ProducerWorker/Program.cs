using EventsDispatcher.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProducerWorker.Services;
using ProducerWorker.Services.Interfaces;
using System.Linq;

namespace ProducerWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var argsList = args.ToList();
                    var productionIntervalStr = argsList.FirstOrDefault(a => a.StartsWith("interval"))?.Split("=").ElementAtOrDefault(1);
                    var isValidInterval = int.TryParse(productionIntervalStr, out var productionInterval);
                    var queueName = argsList.FirstOrDefault(a => a.StartsWith("queue"))?.Split("=").ElementAtOrDefault(1);
                    var workerName = argsList.FirstOrDefault(a => a.StartsWith("worker"))?.Split("=").ElementAtOrDefault(1) ?? "Noname";

                    services.AddSingleton<IProducerWorkerConfigProvider>(
                        new ProducerWorkerConfigProvider(queueName, 
                        string.IsNullOrEmpty(productionIntervalStr) && isValidInterval
                        ? default 
                        : productionInterval,
                        workerName));

                    services.RegisterEventsDispatcher(hostContext.Configuration);
                    services.AddHostedService<ProducerServiceWorker>();
                });
    }
}
