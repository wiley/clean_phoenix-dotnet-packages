using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using FluentAssertions;
using KafkaMessengerDemoApi.LearnerApi.Models;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WLS.KafkaMessenger.Infrastructure;
using WLS.KafkaMessenger.Infrastructure.Interface;
using WLS.KafkaMessenger.Services;
using WLS.KafkaMessenger.Services.Interfaces;
using Xunit;

namespace WLS.KafkaMessenger.Tests
{
    public class KafkaMessengerServiceTests
	{
		private readonly IKafkaMessengerService _kafkaMessengerService;
		private readonly IKafkaConfig _kafkaConfig;
		private readonly IProducer<string, string> _producer;
		private readonly string _host = "testkafkahost";
		private readonly string _topic = "KafkaMessenger";

		public KafkaMessengerServiceTests()
		{
			_kafkaConfig = new KafkaConfig
			{
				Host = _host,
				Sender = new List<KafkaSender>
				{
					new KafkaSender { Topic = _topic }
				}
			};
			_producer = Substitute.For<IProducer<string, string>>();
			_kafkaMessengerService = new KafkaMessengerService(_kafkaConfig, _producer);
		}

		public LearnerAccount CreateLearnerAccount() => new LearnerAccount
		{
			SiteID = Site.MyED,
			AccountID = 1,
			Email = "jbolt@inscapepublishing.com",
			FirstName = "Jeff",
			LastName = "Bolt",
			CreatedDT = DateTime.Now,
			LastUpdateDT = DateTime.Now,
			Searchable = true,
			PasswordChangedDT = DateTime.Now,
			DataConsentDT = DateTime.Now,
			LastLoginDT = DateTime.Now,
			Password = "password",
			Salt = "salt",
			PublicProfileSearch = true,
			CompanyProfileSearch = true,
			AvatarFileName = "85a839960ea24e1a913054176d3c235f.jpg",
			OrganizationID = 693166,  // 693166=Inscape Publishing LLC
			LocationID = 2,  // 1=Waco 2=Minneapolis
			DepartmentID = 6,  // 6=WebDEV
			CountryID = 6252001,  // 6252001=US
			RegionID = 5037779,  // 5037779=Minnesota
			HasCatalystData = true,
			LastUpdateReason = "KafkaMessenger Test"
		};

		[Fact]
		public void SendKafkaMessage_ReturnsSuccess()
		{
			var learnerAccount = CreateLearnerAccount();
			var deliveryResult = new DeliveryResult<string, string>
			{
				Offset = new Offset(1),
				Partition = new Partition(1),
				Topic = _topic,
				TopicPartitionOffset = new TopicPartitionOffset(
					new TopicPartition("Test Topic Partition", new Partition(2)), new Offset(3)),
				Status = PersistenceStatus.Persisted
			};

			var expected = new List<ReturnValue>
			{
				new ReturnValue
				{
					Status = ReturnStatus.Success,
					DeliveryResult = deliveryResult
				}
			};

			_producer.ProduceAsync(Arg.Any<string>(), Arg.Any<Message<string, string>>()).ReturnsForAnyArgs(deliveryResult);

			using (var task = _kafkaMessengerService.SendKafkaMessage(learnerAccount.AccountID.ToString(), "UpdateLearner", learnerAccount))
			{
				task.Wait();

				_producer.Received(1).ProduceAsync(Arg.Any<string>(), Arg.Any<Message<string, string>>());

				Assert.IsType<Task<List<ReturnValue>>>(task);
				Assert.NotNull(task);
				Assert.True(task.Id > 0);
				Assert.True(task.IsCompleted);
				Assert.True(task.IsCompletedSuccessfully);
				Assert.Equal(TaskStatus.RanToCompletion, task.Status);
				Assert.False(task.IsCanceled);
				Assert.False(task.IsFaulted);
				Assert.Null(task.Exception);

				var returnValue = task.Result;
				Assert.IsType<List<ReturnValue>>(returnValue);
				Assert.NotNull(returnValue);
				expected.Should().BeEquivalentTo(returnValue);
			}
		}

		[Fact]
		public void SendKafkaMessage_ProducerException_ReturnsErrorUnknown()
		{
			var learnerAccount = CreateLearnerAccount();
			var exception = new Exception("Test Exception");
			var expected = new List<ReturnValue>
			{
				new ReturnValue
				{
					Status = ReturnStatus.ErrorUnknown,
					Exception = exception
				}
			};

			_producer.ProduceAsync(Arg.Any<string>(), Arg.Any<Message<string, string>>()).ThrowsForAnyArgs(exception);

			using (var task = _kafkaMessengerService.SendKafkaMessage(learnerAccount.AccountID.ToString(), "UpdateLearner", learnerAccount))
			{
				task.Wait();

				_producer.Received(1).ProduceAsync(Arg.Any<string>(), Arg.Any<Message<string, string>>());

				Assert.IsType<Task<List<ReturnValue>>>(task);
				Assert.NotNull(task);
				Assert.True(task.Id > 0);
				Assert.True(task.IsCompleted);
				Assert.True(task.IsCompletedSuccessfully);
				Assert.Equal(TaskStatus.RanToCompletion, task.Status);
				Assert.False(task.IsCanceled);
				Assert.False(task.IsFaulted);
				Assert.Null(task.Exception);

				var returnValue = task.Result;
				Assert.IsType<List<ReturnValue>>(returnValue);
				Assert.NotNull(returnValue);
				expected.Should().BeEquivalentTo(returnValue);
			}
		}
	}
}
