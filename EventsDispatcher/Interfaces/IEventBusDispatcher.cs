using System.Threading.Tasks;
using EventsDispatcher.Models.Abstract;

namespace EventsDispatcher.Interfaces
{
    public interface IEventBusDispatcher
    {
        void Publish(string message, string queue, string routingKey);
        void Publish<TEvent>(TEvent @event)
            where TEvent : EventBase;

        Task PublishAsync(string message, string queue, string routingKey);
        Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : EventBase;
    }
}