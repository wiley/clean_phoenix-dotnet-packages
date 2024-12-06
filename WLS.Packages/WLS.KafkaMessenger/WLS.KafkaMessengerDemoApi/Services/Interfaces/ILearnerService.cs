using KafkaMessengerDemoApi.LearnerApi.Models;

using System.Collections.Generic;

using WLS.KafkaMessenger;

namespace KafkaMessengerDemoApi
{
	public interface ILearnerService
	{
		List<ReturnValue> SendLearnerUpdatedMessage(LearnerAccount learnerAccount);
	}
}