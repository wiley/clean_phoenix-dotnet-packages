using System.Collections.Generic;
using WLS.KafkaProcessor.Domain;

namespace WLS.KafkaProcessor.Services.Interfaces
{
    public interface IMappedServices
    {
        Dictionary<string, MessageTypeServiceMap> Services { get; set; }
    }
}
