using Amazon;
using Amazon.S3;
using MongoDB.Driver;
using System;
using WLS.Monitoring.HealthCheck.Interfaces;
using WLS.Monitoring.HealthCheck.Models;

namespace WLS.Monitoring.HealthCheck
{
    public class DbHealthCheckUtilities : IDbHealthCheckUtilities
    {
        #region MongoDb
        public MongoClient GenerateMongoClient(string connectionString)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return new MongoClient(connectionString);
            }
            return null;
        }
        #endregion

        #region AmazonS3
        public AmazonS3Client GenerateAmazonS3Client(AmazonS3ConnectionParams connectionParams)
        {
            RegionEndpoint regionEndpoint = RegionEndpoint.GetBySystemName(connectionParams.AWS_S3_REGION_NAME);
            return new AmazonS3Client(connectionParams.AWS_S3_ACCESS_KEY_ID, connectionParams.AWS_S3_SECRET_ACCESS_KEY, regionEndpoint);
        }

        public static void AmazonS3CheckAndPopulateConnectionParams(string connectionString, AmazonS3ConnectionParams connectionParam)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                string[] databaseDetails = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
                if (databaseDetails.Length == 4)
                {
                    foreach (string partialConnectionString in databaseDetails)
                    {
                        PopulateAmazonS3ConnectionParams(partialConnectionString, connectionParam);
                    }
                }
            }
        }

        public static void PopulateAmazonS3ConnectionParams(string partialConnectionString, AmazonS3ConnectionParams connectionParams)
        {
            if (!string.IsNullOrWhiteSpace(partialConnectionString))
            {
                if (CompareStringWithParamStructure(partialConnectionString, AmazonS3ConnectionParams.accessKeyIDStringIdentifier))
                {
                    connectionParams.AWS_S3_ACCESS_KEY_ID = GetStringValue(partialConnectionString, AmazonS3ConnectionParams.accessKeyIDStringIdentifier.Length);
                    return;
                }
                if (CompareStringWithParamStructure(partialConnectionString, AmazonS3ConnectionParams.regionStringIdentifier))
                {
                    connectionParams.AWS_S3_REGION_NAME = GetStringValue(partialConnectionString, AmazonS3ConnectionParams.regionStringIdentifier.Length);
                    return;
                }
                if (CompareStringWithParamStructure(partialConnectionString, AmazonS3ConnectionParams.bucketStringIdentifier))
                {
                    connectionParams.AWS_S3_BUCKET_NAME = GetStringValue(partialConnectionString, AmazonS3ConnectionParams.bucketStringIdentifier.Length);
                    return;
                }
                if (CompareStringWithParamStructure(partialConnectionString, AmazonS3ConnectionParams.secretKeyStringIdentifier))
                {
                    connectionParams.AWS_S3_SECRET_ACCESS_KEY = GetStringValue(partialConnectionString, AmazonS3ConnectionParams.secretKeyStringIdentifier.Length);
                    return;
                }
            }
        }

        private static bool CompareStringWithParamStructure(string input, string target)
        {
            return input.ToUpper().StartsWith(target);
        }

        private static string GetStringValue(string target, int lenght)
        {
            int size = target.Length - lenght;
            char[] servername = new char[size];
            target.CopyTo(lenght, servername, 0, size);
            return new string(servername);
        }

        #endregion
    }
}
