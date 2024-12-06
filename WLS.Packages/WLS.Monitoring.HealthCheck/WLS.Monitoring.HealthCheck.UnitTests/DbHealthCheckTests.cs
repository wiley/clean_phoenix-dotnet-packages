using Amazon.S3;
using MongoDB.Driver;
using NSubstitute;
using System;
using System.Data.SqlClient;
using WLS.Monitoring.HealthCheck.Interfaces;
using WLS.Monitoring.HealthCheck.Models;
using WLS.Monitoring.HealthCheck.Service;
using Xunit;

namespace WLS.Monitoring.HealthCheck.UnitTests
{
    public class DbHealthCheckTests
    {
        readonly IDbHealthCheck _dbhealthcheck;
        readonly IDbConnectService _dbConnectService;
        readonly IDbHealthCheckUtilities _dbHealthCheckUtilities;

        public  DbHealthCheckTests()
        {
            _dbHealthCheckUtilities = Substitute.For<IDbHealthCheckUtilities>();
            _dbConnectService = Substitute.For<IDbConnectService>();
            _dbhealthcheck = new DbHealthCheck(_dbConnectService, _dbHealthCheckUtilities);
        }

        #region SqlServerConnectionTest
        [Fact]
        public void SqlServerConnectionTest_ConnectionString_Success()
        {
            string connectionString = "Server=1;Database=2;uid=3;pwd=4;";
            _dbConnectService.ConnectToSqlServerDB(Arg.Any<SqlConnectionStringBuilder>()).Returns(true);

            var resultString = _dbhealthcheck.SqlServerConnectionTest(connectionString);

            Assert.True(resultString.SuccessfulConnection);
        }

        [Fact]
        public void SqlServerConnectionTest_ConnectionParams_Success()
        {
            _dbConnectService.ConnectToSqlServerDB(Arg.Any<SqlConnectionStringBuilder>()).Returns(true);

            SqlConnectionParams inputParam = new SqlConnectionParams()
            {
                Server = "1",
                Database = "2",
                Uid = "3",
                Pwd = "4"
            };
            var resultParams = _dbhealthcheck.SqlServerConnectionTest(inputParam);
            Assert.True(resultParams.SuccessfulConnection);
        }

        [Fact]
        public void SqlServerConnectionTest_ErrorFromIncorrectInput()
        {
            string incorrect = "Server=1;Database=2;uid=3;pwd=4;";

            _dbConnectService.ConnectToSqlServerDB(Arg.Any<SqlConnectionStringBuilder>()).Returns(x => { throw new Exception(); });

            var resultIncorrect = _dbhealthcheck.SqlServerConnectionTest(incorrect);
            Assert.False(resultIncorrect.SuccessfulConnection);
        }

        [Fact]
        public void SqlServerConnectionTest_ConnectionString_ErrorFromNullInput()
        {
            string nullable = null;
            _dbConnectService.ConnectToSqlServerDB(Arg.Any<SqlConnectionStringBuilder>()).Returns(x => { throw new Exception(); });

            var resultNullable = _dbhealthcheck.SqlServerConnectionTest(nullable);

            Assert.False(resultNullable.SuccessfulConnection);
        }

        [Fact]
        public void SqlServerConnectionTest_ConnectionParams_ErrorFromNullInput()
        {
            SqlConnectionParams inputParamNull = null;
            _dbConnectService.ConnectToSqlServerDB(Arg.Any<SqlConnectionStringBuilder>()).Returns(x => { throw new Exception(); });
            
            var resultsParamNull = _dbhealthcheck.SqlServerConnectionTest(inputParamNull);
            Assert.False(resultsParamNull.SuccessfulConnection);
        }

        [Fact]
        public void SqlServerConnectionTest_ConnectionString_ErrorFromEmptyInput()
        {
            string empty = "";

            _dbConnectService.ConnectToSqlServerDB(Arg.Any<SqlConnectionStringBuilder>()).Returns(x => { throw new Exception(); });

            var resultEmpty = _dbhealthcheck.SqlServerConnectionTest(empty);
            Assert.False(resultEmpty.SuccessfulConnection);
        }

        [Fact]
        public void SqlServerConnectionTest_ConnectionParams_ErrorFromEmptyInput()
        {
            SqlConnectionParams inputParamEmpty = new SqlConnectionParams() { };

            _dbConnectService.ConnectToSqlServerDB(Arg.Any<SqlConnectionStringBuilder>()).Returns(x => { throw new Exception(); });

            var resultParamsEmpty = _dbhealthcheck.SqlServerConnectionTest(inputParamEmpty);
            Assert.False(resultParamsEmpty.SuccessfulConnection);
        }

        #endregion

        #region MongoDbConnectionTest
        [Fact()]
        public void MongoDbConnectionTest_Success()
        {
            string connectionString = "connectionStringValue";

            _dbConnectService.ConnectToMongoDb(Arg.Any<MongoClient>()).Returns(new DbHealthCheckResponse(true,""));
            _dbHealthCheckUtilities.GenerateMongoClient(Arg.Any<string>()).Returns(new MongoClient());

            DbHealthCheckResponse result = _dbhealthcheck.MongoDbConnectionTest(connectionString);
            Assert.True(result.SuccessfulConnection);
        }

        [Fact]
        public void MongoDbConnectionTest_ErrorFromNoDbToList()
        {
            string connectionString = "connectionStringValue";
            _dbConnectService.ConnectToMongoDb(Arg.Any<MongoClient>()).Returns(new DbHealthCheckResponse(false, ""));
            
            DbHealthCheckResponse resultIncorrect = _dbhealthcheck.MongoDbConnectionTest(connectionString);

            Assert.False(resultIncorrect.SuccessfulConnection);
        }

        [Fact]
        public void MongoDbConnectionTest_ErrorFromIncorrectInput()
        {
            string incorrect = "something";
            DbHealthCheckResponse resultIncorrect = _dbhealthcheck.MongoDbConnectionTest(incorrect);
            Assert.False(resultIncorrect.SuccessfulConnection);
        }

        [Fact]
        public void MongoDbConnectionTest_ErrorFromNullInput()
        {
            string nullable = null;
            DbHealthCheckResponse resultNullable = _dbhealthcheck.MongoDbConnectionTest(nullable);
            Assert.False(resultNullable.SuccessfulConnection);
        }

        [Fact]
        public void MongoDbConnectionTest_ErrorFromEmptyInput()
        {
            string empty = "";
            DbHealthCheckResponse resultEmpty = _dbhealthcheck.MongoDbConnectionTest(empty);
            Assert.False(resultEmpty.SuccessfulConnection);
        }
        #endregion

        #region GenerateMongoClient
        [Fact]
        public void GenerateMongoClient_ErrorFromNullINput()
        {
            string nullable = null;
            MongoClient resultNullable = _dbHealthCheckUtilities.GenerateMongoClient(nullable);
            Assert.True(resultNullable == null);
        }

        [Fact]
        public void GenerateMongoClient_ErrorFromEmptyInput()
        {
            string empty = "";
            MongoClient resultEmpty = _dbHealthCheckUtilities.GenerateMongoClient(empty);
            Assert.True(resultEmpty == null);
        }
        #endregion

        #region MySqlConnectionTest
        [Fact]
        public void MySqlConnectionTest_ConnectionString_Success()
        {
            string connectionString = "server=1;port=2;database=3;user id=4;password=5";
            _dbConnectService.ConnectToMySql(Arg.Any<string>()).Returns(new DbHealthCheckResponse(true,""));

            var resultString = _dbhealthcheck.MySqlConnectionTest(connectionString);

            Assert.True(resultString.SuccessfulConnection);
        }

        [Fact]
        public void MySqlConnectionTest_ConnectionParams_Success()
        {
            MySqlConnectionParams connectionParam = new MySqlConnectionParams()
            {
                Server = "localhost",
                Port = 3306,
                Database = "world",
                UserId = "root",
                Password = "admin"
            };

            _dbConnectService.ConnectToMySql(Arg.Any<string>()).Returns(new DbHealthCheckResponse(true, ""));

            var resultParams = _dbhealthcheck.MySqlConnectionTest(connectionParam);

            Assert.True(resultParams.SuccessfulConnection);
        }

        [Fact]
        public void MySqlConnectionTest_ConnectionString_ErrorFromIncorrectInput()
        {
            string incorrect = "something";

            IDbHealthCheckUtilities dbHealthCheckUtilities = new DbHealthCheckUtilities();
            DbConnectService dbConnectService = new DbConnectService();
            DbHealthCheck dbhealthcheck = new DbHealthCheck(dbConnectService, dbHealthCheckUtilities);

            DbHealthCheckResponse resultincorrect = dbhealthcheck.MySqlConnectionTest(incorrect);
            Assert.False(resultincorrect.SuccessfulConnection);
        }

        [Fact]
        public void MySqlConnectionTest_ConnectionParams_ErrorFromIncorrectInput()
        {
            IDbHealthCheckUtilities dbHealthCheckUtilities = new DbHealthCheckUtilities();
            DbConnectService dbConnectService = new DbConnectService();
            DbHealthCheck dbhealthcheck = new DbHealthCheck(dbConnectService, dbHealthCheckUtilities);

            MySqlConnectionParams paramIncorrect = new MySqlConnectionParams()
            {
                Server = "1",
                Port = 2,
                Database = "3",
                UserId = "4",
                Password = "5"
            };

            var restulsparamIncorrect = dbhealthcheck.MySqlConnectionTest(paramIncorrect);
            Assert.False(restulsparamIncorrect.SuccessfulConnection);
        }

        [Fact]
        public void MySqlConnectionTest_ConnectionString_ErrorFromNullInput()
        {
            string nullable = null;

            _dbConnectService.ConnectToMySql(Arg.Any<string>()).Returns(new DbHealthCheckResponse(false, ""));

            DbHealthCheckResponse resultNullable = _dbhealthcheck.MySqlConnectionTest(nullable);
            Assert.False(resultNullable.SuccessfulConnection);
        }

        [Fact]
        public void MySqlConnectionTest_ConnectionParams_ErrorFromNullInput()
        {
            MySqlConnectionParams paramNull = null;

            _dbConnectService.ConnectToMySql(Arg.Any<string>()).Returns(new DbHealthCheckResponse(false, ""));

            var resultsParamNull = _dbhealthcheck.MySqlConnectionTest(paramNull);
            Assert.False(resultsParamNull.SuccessfulConnection);
        }

        [Fact]
        public void MySqlConnectionTest_ConnectionString_ErrorFromEmptyInput()
        {
            string empty = "";

            _dbConnectService.ConnectToMySql(Arg.Any<string>()).Returns(new DbHealthCheckResponse(false, ""));

            DbHealthCheckResponse resultEmpty = _dbhealthcheck.MySqlConnectionTest(empty);

            Assert.False(resultEmpty.SuccessfulConnection);
        }

        [Fact]
        public void MySqlConnectionTest_ConnectionParams_ErrorFromEmptyInput()
        {
            MySqlConnectionParams paramEmpty = new MySqlConnectionParams() { };

            _dbConnectService.ConnectToMySql(Arg.Any<string>()).Returns(new DbHealthCheckResponse(false, ""));

            var resultParamsEmpty = _dbhealthcheck.MySqlConnectionTest(paramEmpty);
            Assert.False(resultParamsEmpty.SuccessfulConnection);
        }
        #endregion

        #region AmazonS3ConnectionTest
        [Fact]
        public async void AmazonS3ConnectionTest_ConnectionString_Success()
        {
            string connectionString = "AWS_S3_BUCKET_NAME:1;AWS_S3_REGION_NAME:2;" +
                "AWS_S3_ACCESS_KEY_ID:3;AWS_S3_SECRET_ACCESS_KEY:4";

            _dbConnectService.GetAmazonS3BucketLocation(Arg.Any<AmazonS3Client>(),Arg.Any<string>()).Returns(new DbHealthCheckResponse(true,""));

            var resultString =  await _dbhealthcheck.AmazonS3ConnectionTest(connectionString);
            Assert.True(resultString.SuccessfulConnection);
        }

        [Fact]
        public async void AmazonS3ConnectionTest_ConnectionParams_Success()
        {
            AmazonS3ConnectionParams connectionParam = new AmazonS3ConnectionParams()
            {
                AWS_S3_BUCKET_NAME = "1",
                AWS_S3_REGION_NAME = "2",
                AWS_S3_ACCESS_KEY_ID = "3",
                AWS_S3_SECRET_ACCESS_KEY = "4"
            };

            _dbConnectService.GetAmazonS3BucketLocation(Arg.Any<AmazonS3Client>(), Arg.Any<string>()).Returns(new DbHealthCheckResponse(true, ""));

            var resultParam = await _dbhealthcheck.AmazonS3ConnectionTest(connectionParam);
            Assert.True(resultParam.SuccessfulConnection);
        }

        [Fact]
        public async void AmazonS3ConnectionTest_ConnectionString_ErrorFromIncorrectInput()
        {
            string incorrect = "something";

            IDbHealthCheckUtilities dbHealthCheckUtilities = new DbHealthCheckUtilities();
            DbConnectService dbConnectService = new DbConnectService();
            DbHealthCheck dbhealthcheck = new DbHealthCheck(dbConnectService, dbHealthCheckUtilities);

            var resultIncorrect = await dbhealthcheck.AmazonS3ConnectionTest(incorrect);
            Assert.False(resultIncorrect.SuccessfulConnection);
        }

        [Fact]
        public async void AmazonS3ConnectionTest_ConnectionString_ErrorFromNullInput()
        {
            string nullable = null;

            IDbHealthCheckUtilities dbHealthCheckUtilities = new DbHealthCheckUtilities();
            DbConnectService dbConnectService = new DbConnectService();
            DbHealthCheck dbhealthcheck = new DbHealthCheck(dbConnectService, dbHealthCheckUtilities);

            var resultNullable = await dbhealthcheck.AmazonS3ConnectionTest(nullable);
            Assert.False(resultNullable.SuccessfulConnection);
        }

        [Fact]
        public async void AmazonS3ConnectionTest_ConnectionParams_ErrorFromNullInput()
        {
            IDbHealthCheckUtilities dbHealthCheckUtilities = new DbHealthCheckUtilities();
            DbConnectService dbConnectService = new DbConnectService();
            DbHealthCheck dbhealthcheck = new DbHealthCheck(dbConnectService, dbHealthCheckUtilities);

            AmazonS3ConnectionParams paramNull = null;

            var resultParamNull = await dbhealthcheck.AmazonS3ConnectionTest(paramNull);
            Assert.False(resultParamNull.SuccessfulConnection);
        }

        [Fact]
        public async void AmazonS3ConnectionTest_ConnectionString_ErrorFromEmptyInput()
        {
            string empty = "";

            IDbHealthCheckUtilities dbHealthCheckUtilities = new DbHealthCheckUtilities();
            DbConnectService dbConnectService = new DbConnectService();
            DbHealthCheck dbhealthcheck = new DbHealthCheck(dbConnectService, dbHealthCheckUtilities);

            var resultEmpty = await dbhealthcheck.AmazonS3ConnectionTest(empty);
            Assert.False(resultEmpty.SuccessfulConnection);
        }

        [Fact]
        public async void AmazonS3ConnectionTest_ConnectionParams_ErrorFromEmptyInput()
        {
            AmazonS3ConnectionParams paramEmpty = new AmazonS3ConnectionParams();

            IDbHealthCheckUtilities dbHealthCheckUtilities = new DbHealthCheckUtilities();
            DbConnectService dbConnectService = new DbConnectService();
            DbHealthCheck dbhealthcheck = new DbHealthCheck(dbConnectService, dbHealthCheckUtilities);

            var resultParamEmpty = await dbhealthcheck.AmazonS3ConnectionTest(paramEmpty);
            Assert.False(resultParamEmpty.SuccessfulConnection);
        }
        #endregion

        #region AmazonS3StringCheck
        [Fact]
        public void AmazonS3StringCheck_Success()
        {
            string connectionString = "AWS_S3_BUCKET_NAME:1;AWS_S3_REGION_NAME:2;" +
                "AWS_S3_ACCESS_KEY_ID:3;AWS_S3_SECRET_ACCESS_KEY:4;";
            AmazonS3ConnectionParams expected = new AmazonS3ConnectionParams()
            {
                AWS_S3_BUCKET_NAME = "1",
                AWS_S3_REGION_NAME = "2",
                AWS_S3_ACCESS_KEY_ID = "3",
                AWS_S3_SECRET_ACCESS_KEY = "4"
            };
            AmazonS3ConnectionParams result = new AmazonS3ConnectionParams();
            DbHealthCheckUtilities.AmazonS3CheckAndPopulateConnectionParams(connectionString, result);
            Assert.Equal(expected.AWS_S3_ACCESS_KEY_ID, result.AWS_S3_ACCESS_KEY_ID);
            Assert.Equal(expected.AWS_S3_BUCKET_NAME, result.AWS_S3_BUCKET_NAME);
            Assert.Equal(expected.AWS_S3_SECRET_ACCESS_KEY, result.AWS_S3_SECRET_ACCESS_KEY);
            Assert.Equal(expected.AWS_S3_REGION_NAME, result.AWS_S3_REGION_NAME);
        }

        [Fact]
        public void AmazonS3StringCheck_ErrorFromIncorrectInput()
        {
            string connectionString = "Something";
            AmazonS3ConnectionParams expected = new AmazonS3ConnectionParams();
            AmazonS3ConnectionParams result = new AmazonS3ConnectionParams();

            DbHealthCheckUtilities.AmazonS3CheckAndPopulateConnectionParams(connectionString, result);

            Assert.Equal(expected.AWS_S3_ACCESS_KEY_ID, result.AWS_S3_ACCESS_KEY_ID);
            Assert.Equal(expected.AWS_S3_BUCKET_NAME, result.AWS_S3_BUCKET_NAME);
            Assert.Equal(expected.AWS_S3_SECRET_ACCESS_KEY, result.AWS_S3_SECRET_ACCESS_KEY);
            Assert.Equal(expected.AWS_S3_REGION_NAME, result.AWS_S3_REGION_NAME);
        }

        [Fact]
        public void AmazonS3StringCheck_ErrorFromNullInput()
        {
            string nullable = null;
            AmazonS3ConnectionParams notExpected = new AmazonS3ConnectionParams() {};
            AmazonS3ConnectionParams resultNullble = new AmazonS3ConnectionParams();

            DbHealthCheckUtilities.AmazonS3CheckAndPopulateConnectionParams(nullable, resultNullble);

            Assert.NotSame(notExpected, resultNullble);
        }

        [Fact]
        public void AmazonS3StringCheck_ErrorFromEmptyInput()
        {
            string empty = "";
            AmazonS3ConnectionParams notExpected = new AmazonS3ConnectionParams();
            AmazonS3ConnectionParams resultEmpty = new AmazonS3ConnectionParams();

            DbHealthCheckUtilities.AmazonS3CheckAndPopulateConnectionParams(empty, resultEmpty);

            Assert.NotSame(notExpected, resultEmpty);
        }
        #endregion

        #region PopulateAmazonS3ConnectionParams
        [Fact]
        public void PopulateAmazonS3ConnectionParams_Success()
        {
            string[] details = { "AWS_S3_BUCKET_NAME:1",
                "AWS_S3_REGION_NAME:2",
                "AWS_S3_ACCESS_KEY_ID:3",
                "AWS_S3_SECRET_ACCESS_KEY:4"
            };
            AmazonS3ConnectionParams builder = new AmazonS3ConnectionParams();
            DbHealthCheckUtilities.PopulateAmazonS3ConnectionParams(details[0], builder);
            Assert.Equal("1", builder.AWS_S3_BUCKET_NAME);
            DbHealthCheckUtilities.PopulateAmazonS3ConnectionParams(details[1], builder);
            Assert.Equal("2", builder.AWS_S3_REGION_NAME);
            DbHealthCheckUtilities.PopulateAmazonS3ConnectionParams(details[2], builder);
            Assert.Equal("3", builder.AWS_S3_ACCESS_KEY_ID);
            DbHealthCheckUtilities.PopulateAmazonS3ConnectionParams(details[3], builder);
            Assert.Equal("4", builder.AWS_S3_SECRET_ACCESS_KEY);
        }

        [Fact]
        public void PopulateAmazonS3ConnectionParams_ConnectionString_ErrorFromNullINput()
        {
            string details = null;
            AmazonS3ConnectionParams expected = new AmazonS3ConnectionParams();
            AmazonS3ConnectionParams resultEmpty = new AmazonS3ConnectionParams();

            DbHealthCheckUtilities.PopulateAmazonS3ConnectionParams(details, resultEmpty);

            Assert.Equal(expected.AWS_S3_ACCESS_KEY_ID, resultEmpty.AWS_S3_ACCESS_KEY_ID);
            Assert.Equal(expected.AWS_S3_BUCKET_NAME, resultEmpty.AWS_S3_BUCKET_NAME);
            Assert.Equal(expected.AWS_S3_REGION_NAME, resultEmpty.AWS_S3_REGION_NAME);
            Assert.Equal(expected.AWS_S3_SECRET_ACCESS_KEY, resultEmpty.AWS_S3_SECRET_ACCESS_KEY);
        }

        [Fact]
        public void PopulateAmazonS3ConnectionParams_ConnectionParams_ErrorFromNullINput()
        {
            string details = null;
            AmazonS3ConnectionParams resultNullable = null;

            DbHealthCheckUtilities.PopulateAmazonS3ConnectionParams(details, resultNullable);

            Assert.Null(resultNullable);
        }

        [Fact]
        public void PopulateAmazonS3ConnectionParams_ErrorFromEmptyInput()
        {
            string empty = "";
            AmazonS3ConnectionParams expected = new AmazonS3ConnectionParams();
            AmazonS3ConnectionParams resultEmpty = new AmazonS3ConnectionParams();

            DbHealthCheckUtilities.PopulateAmazonS3ConnectionParams(empty, resultEmpty);

            Assert.Equal(expected.AWS_S3_ACCESS_KEY_ID, resultEmpty.AWS_S3_ACCESS_KEY_ID);
            Assert.Equal(expected.AWS_S3_BUCKET_NAME, resultEmpty.AWS_S3_BUCKET_NAME);
            Assert.Equal(expected.AWS_S3_REGION_NAME, resultEmpty.AWS_S3_REGION_NAME);
            Assert.Equal(expected.AWS_S3_SECRET_ACCESS_KEY, resultEmpty.AWS_S3_SECRET_ACCESS_KEY);
        }
        #endregion

    }
}
