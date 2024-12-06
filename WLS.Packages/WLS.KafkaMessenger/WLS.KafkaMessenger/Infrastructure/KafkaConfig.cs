using WLS.KafkaMessenger.Infrastructure.Interface;
using System.Collections.Generic;

namespace WLS.KafkaMessenger.Infrastructure
{
    public class KafkaConfig : IKafkaConfig
    {
        public string Host { get; set; }
        public IEnumerable<IKafkaSender> Sender { get; set; }
        public string Source { get; set; }
    }
}