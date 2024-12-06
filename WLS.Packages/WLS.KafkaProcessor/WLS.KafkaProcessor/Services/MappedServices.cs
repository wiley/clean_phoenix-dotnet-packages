using System.Collections.Generic;
using WLS.KafkaProcessor.Domain;
using WLS.KafkaProcessor.Services.Interfaces;

namespace WLS.KafkaProcessor.Services
{
    public class MappedServices : IMappedServices
    {
        public Dictionary<string, MessageTypeServiceMap> Services { get; set; }
    }
}