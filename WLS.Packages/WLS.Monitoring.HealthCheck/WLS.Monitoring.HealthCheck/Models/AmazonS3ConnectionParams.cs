namespace WLS.Monitoring.HealthCheck.Models
{
    public class AmazonS3ConnectionParams
    {
        public AmazonS3ConnectionParams()
        {
            AWS_S3_BUCKET_NAME = "";
            AWS_S3_REGION_NAME = "";
            AWS_S3_ACCESS_KEY_ID= "";
            AWS_S3_SECRET_ACCESS_KEY = "";
        }
        public AmazonS3ConnectionParams(string bucket, string region, string accessKeyId, string secretkey)
        {
            AWS_S3_BUCKET_NAME = bucket;
            AWS_S3_REGION_NAME = region;
            AWS_S3_ACCESS_KEY_ID = accessKeyId;
            AWS_S3_SECRET_ACCESS_KEY = secretkey;
        }
        public string AWS_S3_BUCKET_NAME { get; set; }
        public string AWS_S3_REGION_NAME { get; set; }
        public string AWS_S3_ACCESS_KEY_ID { get; set; }
        public string AWS_S3_SECRET_ACCESS_KEY { get; set; }

        public const string regionStringIdentifier = "AWS_S3_REGION_NAME:";
        public const string bucketStringIdentifier = "AWS_S3_BUCKET_NAME:";
        public const string secretKeyStringIdentifier = "AWS_S3_SECRET_ACCESS_KEY:";
        public const string accessKeyIDStringIdentifier = "AWS_S3_ACCESS_KEY_ID:";
    }
}
