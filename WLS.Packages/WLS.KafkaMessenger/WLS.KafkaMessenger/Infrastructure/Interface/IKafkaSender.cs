namespace WLS.KafkaMessenger.Infrastructure.Interface
{
    public interface IKafkaSender
    {
        string Topic { get; set; }
    }
}