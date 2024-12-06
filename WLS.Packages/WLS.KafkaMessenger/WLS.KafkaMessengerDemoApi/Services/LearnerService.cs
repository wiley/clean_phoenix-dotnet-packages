using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaMessengerDemoApi.LearnerApi.Models;
using WLS.KafkaMessenger;
using WLS.KafkaMessenger.Services.Interfaces;

namespace KafkaMessengerDemoApi
{
    // Test service to emulate calling from LearnerAPI
    public class LearnerService : ILearnerService
	{
		private readonly IKafkaMessengerService _kafkaMessengerService;

		public LearnerService(IKafkaMessengerService kafkaMessengerService)
		{
			_kafkaMessengerService = kafkaMessengerService;
		}

		public List<ReturnValue> SendLearnerUpdatedMessage(LearnerAccount learnerAccount)
		{
			Task<List<ReturnValue>> tasks;

			using (tasks = _kafkaMessengerService.SendKafkaMessage(learnerAccount.AccountID.ToString(), "UpdateLearner", learnerAccount))
			{
				tasks.Wait();
			}

			return tasks.Result;
		}
	}
}
