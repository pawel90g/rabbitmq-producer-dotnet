using System;
using System.Text;
using System.Threading.Tasks;
using EventsDispatcher.Interfaces;
using EventsDispatcher.Models.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventsDispatcher {
    public class EventBusDispatcher : IAsyncEventDispatcher
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

        public async Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : EventBase
        {
            if (!rabbitConfigurationProvider.IsEnabled())
            {
                logger.LogInformation("RabbitMQ integration disabled");
                return;
            }

            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var exchangeCfg = rabbitConfigurationProvider.GetExchangeConfig();

            await Task.Run(() =>
            {
                try
                {
                    var exchangePrefix = string.IsNullOrEmpty(exchangeCfg.NamePrefix) ? "" : $"{exchangeCfg.NamePrefix}_";
                    var exchange = $"{exchangePrefix}{GetExchangeName<TEvent>()}";
                    var channel = rabbitConnection.GetChannel();

                    channel.ExchangeDeclare(exchange, "direct", durable: false, autoDelete: false, arguments: null);

                    var body = SerializeEvent(@event);

                    var expiration = @event.Expiration ?? exchangeCfg?.DefaultMessageExpiration;
                    var basicProperties = expiration is null ? null : channel.CreateBasicProperties();
                    if (basicProperties != null)
                        basicProperties.Expiration = expiration.Value.ToString();

                    channel.BasicPublish(exchange: exchange,
                                         routingKey: @event.Severity,
                                         basicProperties: basicProperties,
                                         mandatory: false,
                                         body: body);


                    logger.LogInformation($"Event successfully published to exchange {exchange}");
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