using Amazon.S3;
using MongoDB.Driver;
using WLS.Monitoring.HealthCheck.Models;

namespace WLS.Monitoring.HealthCheck.Interfaces
{
    public interface IDbHealthCheckUtilities
    {
        MongoClient GenerateMongoClient(string connectionString);

        AmazonS3Client GenerateAmazonS3Client(AmazonS3ConnectionParams connectionParams);
    }
}
