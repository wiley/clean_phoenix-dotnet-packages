using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace WLS.Log.LoggerTransactionPattern.Tests
{
    public class LoggerStateFactoryTest
    {
        private readonly Mock<ILogger<LoggerStateFactory>> _logger;

        public LoggerStateFactoryTest()
        {
            _logger = new Mock<ILogger<LoggerStateFactory>>();
        }

        [Test]
        public void Create_ValidTransactionId_ShouldCreateValidState()
        {
            LoggerStateFactory factory = new LoggerStateFactory(_logger.Object);
            Guid validGuid = Guid.NewGuid();
            LoggerState state = factory.Create(validGuid.ToString());
            Assert.AreEqual(validGuid.ToString(), state[LoggerState.TransactionId]);
        }

        [Test]
        public void Create_InvalidTransactionId_ShouldCreateValidState()
        {
            LoggerStateFactory factory = new LoggerStateFactory(_logger.Object);
            LoggerState state = factory.Create(null);

            Assert.IsNotNull(Guid.Parse(state[LoggerState.TransactionId].ToString()));
        }
    }
}
