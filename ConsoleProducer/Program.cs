using System;
using EventsDispatcher.Interfaces;

namespace ConsoleProducer
{
    class Program
    {
        static IServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            serviceProvider = Bootstrapper.GetServiceProvider();

            var queue = args[0];
            var routingKey = args[1];

            var eventBusDispatcher = (IEventBusDispatcher)serviceProvider.GetService(typeof(IEventBusDispatcher));
            
        }
    }
}
