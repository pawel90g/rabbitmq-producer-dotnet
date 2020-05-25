using System.Threading.Tasks;
using EventsDispatcher.Models.Abstract;

namespace EventsDispatcher.Interfaces
{
    public interface IAsyncEventDispatcher
    {
        Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : EventBase;
    }
}