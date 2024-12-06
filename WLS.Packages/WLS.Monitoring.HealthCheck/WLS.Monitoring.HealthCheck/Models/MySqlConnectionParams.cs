namespace WLS.Monitoring.HealthCheck.Models
{
    public class MySqlConnectionParams
    {
        public string Server { get; set; }
        public uint Port { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
