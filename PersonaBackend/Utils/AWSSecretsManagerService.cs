using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace PersonaBackend.Utils
{
    public class AWSSecretsManagerService
    {
        private static AWSSecretsManagerService _instance;
        private readonly IAmazonSecretsManager _client;

        private readonly string _accessKeyId;
        private readonly string _secretAccessKey;
        private readonly string _region;

        private AWSSecretsManagerService(string accessKeyId, string secretAccessKey, string region)
        {
            _accessKeyId = accessKeyId ?? throw new ArgumentNullException(nameof(accessKeyId));
            _secretAccessKey = secretAccessKey ?? throw new ArgumentNullException(nameof(secretAccessKey));
            _region = region ?? throw new ArgumentNullException(nameof(region));

            _client = new AmazonSecretsManagerClient(_accessKeyId, _secretAccessKey, RegionEndpoint.GetBySystemName(_region));
        }

        public static AWSSecretsManagerService Instance
        {
            get
            {
                if (_instance == null)
                {
                    DotNetEnv.Env.Load(".env");
                    var accessKeyId = DotNetEnv.Env.GetString("AWS_ACCESS_KEY_ID");
                    var secretAccessKey = DotNetEnv.Env.GetString("AWS_SECRET_ACCESS_KEY");
                    var region = DotNetEnv.Env.GetString("AWS_REGION");

                    _instance = new AWSSecretsManagerService(accessKeyId, secretAccessKey, region);
                }
                return _instance;
            }
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            try
            {
                var request = new GetSecretValueRequest
                {
                    SecretId = secretName,
                    VersionStage = "AWSCURRENT"
                };

                var response = await _client.GetSecretValueAsync(request);
                return response.SecretString;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving secret '{secretName}' from AWS Secrets Manager: {ex.Message}");
            }
        }
    }
}