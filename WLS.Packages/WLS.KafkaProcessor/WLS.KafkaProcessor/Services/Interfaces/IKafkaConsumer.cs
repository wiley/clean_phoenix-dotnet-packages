using System;
using System.Threading.Tasks;
using Kafka.Public;

namespace WLS.KafkaProcessor.Services.Interfaces
{
    public interface IKafkaConsumer
    {
        Task Consume(RawKafkaRecord record, IMappedServices mappedServices);
    }
}
