using System.Data.SqlClient;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using MongoDB.Driver;
using MySqlConnector;
using WLS.Monitoring.HealthCheck.Interfaces;
using WLS.Monitoring.HealthCheck.Models;

namespace WLS.Monitoring.HealthCheck.Service
{
    public class DbConnectService : IDbConnectService
    {
        public bool ConnectToSqlServerDB(SqlConnectionStringBuilder builder)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
            }
            return true;
        }

        public DbHealthCheckResponse ConnectToMongoDb(MongoClient dbClient)
        {
            var dbList = dbClient.ListDatabases().ToList();
            if(dbList.Count !=0 )
            {
                return new DbHealthCheckResponse(true, "MongoDb connection is working");
            }
            return new DbHealthCheckResponse(false,
               "MongoDb connection could not find any database to list," +
               " either the database is empty of the connection string is incorrect");
        }

        public DbHealthCheckResponse ConnectToMySql(string connectionString)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var connectionState = conn.State;
                return new DbHealthCheckResponse(true, string.Format(
                    "MySql Connection opened, connection state {0}"
                    , connectionState));
            }
        }

        public async Task<DbHealthCheckResponse> GetAmazonS3BucketLocation(AmazonS3Client client, string bucketName)
        {
            GetBucketLocationResponse response = await client.GetBucketLocationAsync(bucketName);
            return new DbHealthCheckResponse(true, string.Format("AmazonS3 bucket found in the following region {0}",
                response.Location.Value));
        }
    }
}
