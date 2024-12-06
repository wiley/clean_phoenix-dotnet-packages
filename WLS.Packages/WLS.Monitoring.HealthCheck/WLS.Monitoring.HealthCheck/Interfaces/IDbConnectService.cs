using Amazon.S3;
using MongoDB.Driver;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WLS.Monitoring.HealthCheck.Models;

namespace WLS.Monitoring.HealthCheck.Interfaces
{
    public interface IDbConnectService
    {
        bool ConnectToSqlServerDB(SqlConnectionStringBuilder builder);
        DbHealthCheckResponse ConnectToMongoDb(MongoClient dbClient);
        DbHealthCheckResponse ConnectToMySql(string connectionString);
        Task<DbHealthCheckResponse> GetAmazonS3BucketLocation(AmazonS3Client client, string bucketName);
    }
}
