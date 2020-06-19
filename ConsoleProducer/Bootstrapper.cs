using System;
using EventsDispatcher.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleProducer {
    public static class Bootstrapper {
        public static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            ConfigureServices(services, configuration);

            return services.BuildServiceProvider();
        }
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration) =>
            services.RegisterEventsDispatcher(configuration);
    }
}