namespace WLS.Monitoring.HealthCheck.Models
{
    public class SqlConnectionParams
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Uid  { get; set; }
        public string Pwd { get; set; }
    }
}
