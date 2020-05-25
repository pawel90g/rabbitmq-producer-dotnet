namespace EventsDispatcher.Interfaces
{
    public interface IRabbitConfigurationProvider
    {
        string GetHostName();
        string GetUserName();
        string GetPassword();
        bool IsEnabled();
        ExchangeConfig GetExchangeConfig();
    }
}