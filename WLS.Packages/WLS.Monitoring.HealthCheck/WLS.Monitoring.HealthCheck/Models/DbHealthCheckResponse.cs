namespace WLS.Monitoring.HealthCheck.Models
{
    public class DbHealthCheckResponse
    {
        public DbHealthCheckResponse(bool connection, string message)
        {
            SuccessfulConnection = connection;
            ResponseMessage = message;
        }
        public bool SuccessfulConnection { get; set; }
        public string ResponseMessage { get; set; }
    }
}
