using ProducerWorker.Services.Interfaces;

namespace ProducerWorker.Services {
    public class ProducerWorkerConfigProvider : IProducerWorkerConfigProvider
    {
        private readonly string workerName;
        private readonly string queueName;
        private readonly int productionInterval;
        public ProducerWorkerConfigProvider(string queueName = "queue", int productionInterval = 1000, string workerName = null)
        {
            this.queueName = queueName;
            this.productionInterval = productionInterval;
            this.workerName = workerName;
        }

        public string GetQueueName() => queueName;
        public int GetProductionInterval() => productionInterval;

        public string GetWorkerName() => workerName;
    }
}