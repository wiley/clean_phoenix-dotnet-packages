namespace WLS.Log.LoggerTransactionPattern
{
    public interface ILoggerStateFactory
    {
        LoggerState Create(string transactionId);
    }
}
