using Newtonsoft.Json;

namespace EventsDispatcher.Models.Abstract
{
    public abstract class EventBase
    {
        [JsonIgnore]
        public string RoutingKey { get; protected set; }
        [JsonIgnore]
        public long? Expiration { get; protected set; }

        private EventBase() { }

        protected EventBase(string severity)
        {
            RoutingKey = severity;
            Expiration = null;
        }

        protected EventBase(string severity, long expiration)
        {
            RoutingKey = severity;
            Expiration = expiration;
        }
    }
}