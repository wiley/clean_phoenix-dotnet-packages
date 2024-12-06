using System.Collections.Generic;

namespace WLS.KafkaMessenger.Infrastructure.Interface
{
    public interface IKafkaConfig
    {
        string Host { get; set; }
        IEnumerable<IKafkaSender> Sender { get; set; }
        string Source { get; set; }
    }
}