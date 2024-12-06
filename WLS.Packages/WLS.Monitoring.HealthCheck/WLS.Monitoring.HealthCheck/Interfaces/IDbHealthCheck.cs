using System.Threading.Tasks;
using WLS.Monitoring.HealthCheck.Models;

namespace WLS.Monitoring.HealthCheck.Interfaces
{
    public interface IDbHealthCheck
    {
        DbHealthCheckResponse SqlServerConnectionTest(string connectionString);
        DbHealthCheckResponse SqlServerConnectionTest(SqlConnectionParams sqlConnectionParams);
        DbHealthCheckResponse MongoDbConnectionTest(string connectionString);
        DbHealthCheckResponse MySqlConnectionTest(string connectionString);
        DbHealthCheckResponse MySqlConnectionTest(MySqlConnectionParams connectionParams);
        Task<DbHealthCheckResponse> AmazonS3ConnectionTest(string connectionString);
        Task<DbHealthCheckResponse> AmazonS3ConnectionTest(AmazonS3ConnectionParams connectionParams);
    }
}
