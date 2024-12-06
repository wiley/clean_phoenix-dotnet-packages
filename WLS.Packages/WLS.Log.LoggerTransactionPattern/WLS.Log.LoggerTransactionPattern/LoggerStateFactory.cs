using Microsoft.Extensions.Logging;
using System;

namespace WLS.Log.LoggerTransactionPattern
{
    public class LoggerStateFactory: ILoggerStateFactory
    {
        private readonly ILogger<LoggerStateFactory> _logger;

        public LoggerStateFactory(ILogger<LoggerStateFactory> logger)
        {
            _logger = logger;
        }

        public LoggerState Create(string transactionId)
        {
            LoggerState loggerState = new LoggerState();
            if (!Guid.TryParse(transactionId, out Guid guid))
            {
                guid = Guid.NewGuid();
                _logger.LogInformation("LoggerStateFactory - Transaction-ID was not found in headers");
            }

            // Transaction ID pattern: https://confluence.wiley.com/display/WPPA/Transaction+ID+pattern
            loggerState.WriteTransactionId(guid);

            return loggerState;
        }
    }
}
