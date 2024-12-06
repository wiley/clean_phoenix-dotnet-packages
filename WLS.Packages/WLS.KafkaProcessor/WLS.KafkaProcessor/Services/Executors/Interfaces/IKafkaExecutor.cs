using System.Threading.Tasks;

namespace WLS.KafkaProcessor.Services.Executors.Interfaces
{
    public interface IKafkaExecutor<T> where T : class
    {
        Task<bool> Execute(T message, string subject);
    }
}
