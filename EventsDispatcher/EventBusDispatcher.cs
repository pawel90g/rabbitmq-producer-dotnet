using System;
using System.Text;
using System.Threading.Tasks;
using EventsDispatcher.Interfaces;
using EventsDispatcher.Models.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace EventsDispatcher
{
    public class EventBusDispatcher : IEventBusDispatcher
    {
        private readonly IRabbitConfigurationProvider rabbitConfigurationProvider;
        private readonly IRabbitConnection rabbitConnection;
        private readonly ILogger<EventBusDispatcher> logger;

        public EventBusDispatcher(
            IRabbitConfigurationProvider rabbitConfigurationProvider,
            IRabbitConnection rabbitConnection,
            ILogger<EventBusDispatcher> logger
            )
        {
            this.logger = logger;
            this.rabbitConfigurationProvider = rabbitConfigurationProvider;
            this.rabbitConnection = rabbitConnection;
        }

        public void Publish(string message, string queue, string routingKey)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            using (var channel = rabbitConnection.GetChannel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
            }
        }

        public void Publish<TEvent>(TEvent @event)
            where TEvent : EventBase
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var exchangeCfg = rabbitConfigurationProvider.GetExchangeConfig();

            var exchange = GetExchangeName<TEvent>();
            var channel = rabbitConnection.GetChannel();

            channel.ExchangeDeclare(exchange, "direct", durable: false, autoDelete: false, arguments: null);

            var body = SerializeEvent(@event);

            var expiration = @event.Expiration ?? exchangeCfg?.DefaultMessageExpiration;
            var basicProperties = expiration is null ? null : channel.CreateBasicProperties();
            if (basicProperties != null)
                basicProperties.Expiration = expiration.Value.ToString();

            channel.BasicPublish(exchange: exchange,
                                 routingKey: @event.RoutingKey,
                                 basicProperties: basicProperties,
                                 mandatory: false,
                                 body: body);
        }

        public Task PublishAsync(string message, string queue, string routingKey)
        {
            return Task.Run(() =>
            {
                try
                {
                    Publish(message, queue, routingKey);
                }
                catch (Exception ex)
                {
                    logger.LogError("Error occures during event publishing", ex);
                }
            });
        }

        public Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : EventBase
        {
            return Task.Run(() =>
            {
                try
                {
                    Publish<TEvent>(@event);
                }
                catch (Exception ex)
                {
                    logger.LogError("Error occures during event publishing", ex);
                }
            });
        }

        private byte[] SerializeEvent(object @object) =>
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@object));

        private TEvent DeserializeEvent<TEvent>(byte[] bytes)
            where TEvent : EventBase =>
            JsonConvert.DeserializeObject<TEvent>(Encoding.UTF8.GetString(bytes));

        private static string GetExchangeName(EventBase @event) => @event.GetType().FullName;
        private static string GetExchangeName<TEvent>() where TEvent : EventBase => typeof(TEvent).FullName;
    }
}