using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace WLS.Log.LoggerTransactionPattern
{
    [Serializable]
    public class LoggerState : Dictionary<string, object>
    {
        public const string TransactionId = "Transaction-ID";

        public LoggerState(): base() { }
        protected LoggerState(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public void WriteTransactionId(Guid id)
        {
            base[TransactionId] = id.ToString();
        }

        public new LoggerState Remove(string key)
        {
            if (key.ToLower() != TransactionId.ToLower())
                base.Remove(key);
            return this;
        }

        public new LoggerState Remove(string key, out object value)
        {
            object baseValue = null;
            if (key.ToLower() != TransactionId.ToLower())
                base.Remove(key, out baseValue);
            value = baseValue;
            return this;
        }

        public new LoggerState Clear()
        {
            foreach (string key in Keys.ToList())
                if (key.ToLower() != TransactionId.ToLower())
                    base.Remove(key);
            return this;
        }
    }
}
