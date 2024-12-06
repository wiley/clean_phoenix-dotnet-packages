using System;
using NUnit.Framework;

namespace WLS.Log.LoggerTransactionPattern.Tests
{
    public class LoggerStateTest
    {

        [Test]
        public void WriteTransactionId_ShouldWriteTheCorrectId()
        {
            Guid guidTest = Guid.NewGuid();
            LoggerState stateTest = new LoggerState();
            stateTest.WriteTransactionId(guidTest);
            Assert.AreEqual(1, stateTest.Count);
            Assert.AreEqual(guidTest.ToString(), stateTest[LoggerState.TransactionId]);
        }

        [Test]
        public void Remove_ShouldNotRemoveTransactionId()
        {
            Guid guidTest = Guid.NewGuid();
            LoggerState stateTest = new LoggerState();
            stateTest.WriteTransactionId(guidTest);
            stateTest["should_be_removed"] = "removed";
            stateTest.Remove(LoggerState.TransactionId);
            stateTest.Remove("should_be_removed");
            Assert.AreEqual(1, stateTest.Count);
            Assert.AreEqual(guidTest.ToString(), stateTest[LoggerState.TransactionId]);
        }

        [Test]
        public void RemoveOut_ShouldNotRemoveTransactionId()
        {
            Guid guidTest = Guid.NewGuid();
            LoggerState stateTest = new LoggerState();
            stateTest.WriteTransactionId(guidTest);
            stateTest["should_be_removed"] = "removed";
            stateTest.Remove(LoggerState.TransactionId, out object transactionOut);
            stateTest.Remove("should_be_removed", out object shouldBeRemovedOut);
            Assert.IsNull(transactionOut);
            Assert.IsNotNull(shouldBeRemovedOut);
            Assert.AreEqual(1, stateTest.Count);
            Assert.AreEqual(guidTest.ToString(), stateTest[LoggerState.TransactionId]);
        }

        [Test]
        public void Clear_ShouldNotRemoveTransactionId()
        {
            Guid guidTest = Guid.NewGuid();
            LoggerState stateTest = new LoggerState();
            stateTest.WriteTransactionId(guidTest);
            stateTest["should_be_removed"] = "removed";
            stateTest.Clear();
            Assert.AreEqual(1, stateTest.Count);
            Assert.AreEqual(guidTest.ToString(), stateTest[LoggerState.TransactionId]);
        }
    }
}
