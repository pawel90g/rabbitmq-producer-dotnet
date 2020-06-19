namespace ProducerWorker.Services.Interfaces {
    public interface IProducerWorkerConfigProvider {

        string GetWorkerName();
        string GetQueueName();
        int GetProductionInterval();
    }
}