using System.Collections.Generic;
using System.Threading.Tasks;

namespace WLS.KafkaMessenger.Services.Interfaces
{
	public interface IKafkaMessengerService
	{
		Task<List<ReturnValue>> SendKafkaMessage(string id, string subject, object message, string topic = "");
	}
}