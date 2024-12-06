using WLS.KafkaMessenger.Infrastructure.Interface;

namespace WLS.KafkaMessenger.Infrastructure
{
    public class KafkaSender : IKafkaSender
    {
        public string Topic { get; set; }
    }
}