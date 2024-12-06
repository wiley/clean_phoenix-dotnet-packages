using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Amazon.S3;
using MongoDB.Driver;
using MySqlConnector;
using WLS.Monitoring.HealthCheck.Interfaces;
using WLS.Monitoring.HealthCheck.Models;
using WLS.Monitoring.HealthCheck.Service;

namespace WLS.Monitoring.HealthCheck
{
    public class DbHealthCheck : IDbHealthCheck
    {
        readonly IDbConnectService dbConnectService;
        readonly IDbHealthCheckUtilities dbHealthCheckUtilities;

        public DbHealthCheck()
        {
            dbConnectService = new DbConnectService();
            dbHealthCheckUtilities = new DbHealthCheckUtilities();
        }
        public DbHealthCheck(IDbConnectService connectService, IDbHealthCheckUtilities dbUtilities)
        {
            dbConnectService = connectService;
            dbHealthCheckUtilities = dbUtilities;
        }

        #region SqlServer
        /// <summary>
        /// This method tries to connect with a Sql server
        /// </summary>
        /// <param name="assessment">String that needs to be in the following format
        /// "Server=1;Database=2;uid=3;pwd=4;"
        /// Replace the numbers with the contents of your environment</param>
        /// <returns>ADbHealthCheckResponse True means the connections was working,
        /// the string should have some details regarding errors and exceptions</returns>
        public DbHealthCheckResponse SqlServerConnectionTest(string connectionString)
        {
            try
            {
                dbConnectService.ConnectToSqlServerDB(new SqlConnectionStringBuilder(connectionString));
            }
            catch (Exception ex)
            {
                return new DbHealthCheckResponse(false, ex.Message);
            }
            return new DbHealthCheckResponse(true, "SqlServer connection is working");
        }

        /// <summary>
        /// This method tries to connect with a Sql server
        /// </summary>
        /// <param name="sqlConnectionParams">An object which contains the params for the connection</param>
        /// <returns>A DbHealthCheckResponse True means the connections was working,
        /// the string should have some details regarding errors and exceptions</returns>
        public DbHealthCheckResponse SqlServerConnectionTest(SqlConnectionParams sqlConnectionParams)
        {
            try
            {
                dbConnectService.ConnectToSqlServerDB(new SqlConnectionStringBuilder()
                {
                    DataSource = sqlConnectionParams.Server,
                    InitialCatalog = sqlConnectionParams.Database,
                    UserID = sqlConnectionParams.Uid,
                    Password = sqlConnectionParams.Pwd
                });
            }
            catch (Exception ex)
            {
                return new DbHealthCheckResponse(false, ex.Message);
            }
            return new DbHealthCheckResponse(true, "SqlServer connection is working");
        }

        #endregion

        #region MongoDB
        /// <summary>
        /// This method tries to connect with a MongoDB server and checks the list of databases
        /// </summary>
        /// <param name="connectionString">A string with the mongoDB connection path</param>
        /// <returns>A DbHealthCheckResponse True means the connections was working,
        /// the string should have some details regarding errors and exceptions</returns>
        public DbHealthCheckResponse MongoDbConnectionTest(string connectionString)
        {
            try
            {
                MongoClient dbClient = dbHealthCheckUtilities.GenerateMongoClient(connectionString);
                if(dbClient != null)
                {
                    return dbConnectService.ConnectToMongoDb(dbClient);
                }
            }
            catch (Exception ex)
            {
                 return new DbHealthCheckResponse(false, ex.Message); ;
            }
            return new DbHealthCheckResponse(false, 
                "Unable to generate MongoDbClient with parameters provided");
        }
        #endregion

        #region MySqlServer
        /// <summary>
        /// This method tries to connect with a MySQL server
        /// </summary>
        /// <param name="connectionString">String that needs to be in the following format
        /// "server=1;port=2;database=3;user id=4;password=5";
        /// Replace the numbers with the contents of your environment</param>
        /// <returns>A DbHealthCheckResponse True means the connections was working,
        /// the string should have some details regarding errors and exceptions,
        /// or state results for working connections</returns>
        public DbHealthCheckResponse MySqlConnectionTest(string connectionString)
        {
            try
            {
                return dbConnectService.ConnectToMySql(connectionString);
            }
            catch (Exception ex)
            {
                return new DbHealthCheckResponse(false, ex.Message);
            }
        }

        /// <summary>
        /// This method tries to connect with a MySQL server
        /// </summary>
        /// <param name="connectionParams">An object which contains the params for the connection</param>
        /// <returns>A DbHealthCheckResponse True means the connections was working,
        /// the string should have some details regarding errors and exceptions,
        /// or state results for working connections</returns>
        public DbHealthCheckResponse MySqlConnectionTest(MySqlConnectionParams connectionParams)
        {
            try
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
                {
                    Server = connectionParams.Server,
                    Database = connectionParams.Database,
                    Port = connectionParams.Port,
                    UserID = connectionParams.UserId,
                    Password = connectionParams.Password
                };

                return dbConnectService.ConnectToMySql(builder.ConnectionString);
            }
            catch (Exception ex)
            {
                return new DbHealthCheckResponse(false, ex.Message);
            }
        }

        #endregion

        #region AmazonS3
        /// <summary>
        /// This method tries to connect with a Amazon S3(Simple Storage Service)
        /// server and check a bucket region
        /// </summary>
        /// <param name="connectionString">A string that needs to be in the following format
        /// "AWS_S3_BUCKET_NAME:bucketValue;AWS_S3_REGION_NAME:regionValue;
        /// AWS_S3_ACCESS_KEY_ID:keyIdValue;AWS_S3_SECRET_ACCESS_KEY:secretKeyValue 
        /// Replace the values with your environment credentials</param>
        /// <returns>A DbHealthCheckResponse True means the connections was working,
        /// the string should have some details regarding errors and exceptions</returns>
        public async Task<DbHealthCheckResponse> AmazonS3ConnectionTest(string connectionString)
        {
            try
            {
                AmazonS3ConnectionParams connectionParams = new AmazonS3ConnectionParams();
                DbHealthCheckUtilities.AmazonS3CheckAndPopulateConnectionParams(connectionString, connectionParams);
                AmazonS3Client client = dbHealthCheckUtilities.GenerateAmazonS3Client(connectionParams);

                return await dbConnectService.GetAmazonS3BucketLocation(client, connectionParams.AWS_S3_BUCKET_NAME);
            }
            catch (Exception ex)
            {
                return new DbHealthCheckResponse (false, ex.Message);
            }
        }

        /// <summary>
        /// This method tries to connect with a Amazon S3(Simple Storage Service)
        /// server and check a bucket region
        /// </summary>
        /// <param name="connectionParams">An object with the connection params</param>
        /// <returns>ADbHealthCheckResponse True means the connections was working,
        /// the string should have some details regarding errors and exceptions</returns>
        public async Task<DbHealthCheckResponse> AmazonS3ConnectionTest(AmazonS3ConnectionParams connectionParams)
        {
            try
            {
                AmazonS3Client client = dbHealthCheckUtilities.GenerateAmazonS3Client(connectionParams);
                return await dbConnectService.GetAmazonS3BucketLocation(client, connectionParams.AWS_S3_BUCKET_NAME);
            }
            catch (Exception ex)
            {
                return new DbHealthCheckResponse(false, ex.Message);
            }
        }

        #endregion
    }
}
