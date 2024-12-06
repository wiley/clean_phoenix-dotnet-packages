using Confluent.Kafka;

using WLS.KafkaMessenger.Domain;
using WLS.KafkaMessenger.Infrastructure.Interface;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using WLS.KafkaMessenger.Infrastructure;
using WLS.KafkaMessenger.Services.Interfaces;

namespace WLS.KafkaMessenger.Services
{
    /* Usage examples:
		https://github.com/confluentinc/confluent-kafka-dotnet
		https://www.csharpcodi.com/vs2/?source=1026%2Fconfluent-kafka-dotnet
	*/
    public class KafkaMessengerService : IKafkaMessengerService
    {
        private readonly IKafkaConfig _kafkaConfig;
        private readonly IProducer<string, string> _producer;

        public KafkaMessengerService(IKafkaConfig kafkaConfig, IProducer<string, string> producer)
        {
            _kafkaConfig = kafkaConfig;
            _producer = producer;
        }

        public async Task<List<ReturnValue>> SendKafkaMessage(string id, string subject, object message, string topic = "")
        {
            var deliveryResults = new List<ReturnValue>();
            var senders = new List<IKafkaSender>();
            if (!String.IsNullOrEmpty(topic))
            {
                senders.Add(
                new KafkaSender
                {
                    Topic = topic
                });
            }
            else
            {
                senders.AddRange(_kafkaConfig.Sender);
            }
            foreach (var sender in senders)
            {
                var result = await Send(id, subject, message, sender.Topic);
                deliveryResults.Add(result);
            }

            _producer.Flush(TimeSpan.FromSeconds(30));

            return deliveryResults;
        }

        private async Task<ReturnValue> Send(string id, string subject, object message, string topic)
        {
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                var jsonMessage = JsonSerializer.Serialize(message, options);

                try
                {
                    var result = await _producer.ProduceAsync(topic, new Message<string, string>()
                    {
                        Key = id,
                        Value = JsonSerializer.Serialize(new KafkaMessage
                        {
                            Type = message.GetType().Name,
                            Data = jsonMessage,
                            DataContentType = "application/json",
                            Time = DateTime.UtcNow,
                            SpecVersion = "1.0",
                            Id = id,
                            Source = _kafkaConfig.Source,
                            Subject = subject
                        })
                    });

                    return new ReturnValue
                    {
                        Status = ReturnStatus.Success,
                        DeliveryResult = result
                    };
                }
                catch (Exception ex)
                {
                    return new ReturnValue
                    {
                        Status = ReturnStatus.ErrorUnknown,
                        Exception = ex
                    };
                }
            };
        }
    }
}