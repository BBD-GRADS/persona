using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using Amazon.Scheduler.Model;
using Amazon.Scheduler;

namespace PersonaBackend.Utils
{
    public class AWSManagerService
    {
        private static AWSManagerService _instance;
        private readonly IAmazonSecretsManager _secretsManagerClient;
        private readonly IAmazonSimpleSystemsManagement _ssmClient;
        private readonly IAmazonScheduler _schedulerClient;

        private readonly string _accessKeyId;
        private readonly string _secretAccessKey;
        private readonly string _region;

        private AWSManagerService(string accessKeyId, string secretAccessKey, string region)
        {
            Console.WriteLine("looking now in prv contsteuctor");
            Console.WriteLine(accessKeyId);
            Console.WriteLine(secretAccessKey);
            Console.WriteLine(region);
            _accessKeyId = accessKeyId ?? throw new ArgumentNullException(nameof(accessKeyId));
            _secretAccessKey = secretAccessKey ?? throw new ArgumentNullException(nameof(secretAccessKey));
            _region = region ?? throw new ArgumentNullException(nameof(region));

            _secretsManagerClient = new AmazonSecretsManagerClient(_accessKeyId, _secretAccessKey, RegionEndpoint.GetBySystemName(_region));
            _ssmClient = new AmazonSimpleSystemsManagementClient(_accessKeyId, _secretAccessKey, RegionEndpoint.GetBySystemName(_region));
            _schedulerClient = new AmazonSchedulerClient(_accessKeyId, _secretAccessKey, RegionEndpoint.GetBySystemName(_region));
        }

        public static AWSManagerService Instance
        {
            get
            {
                if (_instance == null)
                {
                    // DotNetEnv.Env.Load(".env");
                    Console.WriteLine("Retrieving env variables");
                    var accessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
                    var secretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
                    var region = Environment.GetEnvironmentVariable("AWS_REGION");
                    Console.WriteLine(accessKeyId);
                    Console.WriteLine(secretAccessKey);
                    Console.WriteLine(region);

                    _instance = new AWSManagerService(accessKeyId, secretAccessKey, region);
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

                var response = await _secretsManagerClient.GetSecretValueAsync(request);
                return response.SecretString;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving secret '{secretName}' from AWS Secrets Manager: {ex.Message}");
            }
        }

        public async Task PutParameterAsync(string parameterName, string parameterValue)
        {
            try
            {
                var request = new PutParameterRequest
                {
                    Name = parameterName,
                    Value = parameterValue,
                    Type = ParameterType.String,
                    Overwrite = true
                };

                var response = await _ssmClient.PutParameterAsync(request);
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Error putting parameter '{parameterName}' to AWS SSM Parameter Store.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error putting parameter '{parameterName}' to AWS SSM Parameter Store: {ex.Message}");
            }
        }

        public async Task<bool> EnableSchedule(string scheduleName, bool enable = false)
        {
            try
            {
                var getScheduleRequest = new GetScheduleRequest
                {
                    Name = scheduleName
                };

                var getScheduleResponse = await _schedulerClient.GetScheduleAsync(getScheduleRequest);
                var existingSchedule = getScheduleResponse;

                if (existingSchedule == null)
                {
                    throw new Exception($"Schedule '{scheduleName}' does not exist or could not be retrieved.");
                }

                var state = "DISABLED";
                if (enable)
                {
                    state = "ENABLED";
                }
                var startDateUtc = DateTime.UtcNow;
                var endDateUtc = startDateUtc.AddDays(7);

                var updateScheduleRequest = new UpdateScheduleRequest
                {
                    Name = scheduleName,
                    State = state,
                    ScheduleExpression = existingSchedule.ScheduleExpression,
                    Description = existingSchedule.Description,
                    StartDate = startDateUtc,
                    ActionAfterCompletion = existingSchedule.ActionAfterCompletion,
                    EndDate = endDateUtc,
                    FlexibleTimeWindow = existingSchedule.FlexibleTimeWindow,
                    GroupName = existingSchedule.GroupName,
                    KmsKeyArn = existingSchedule.KmsKeyArn,
                    ScheduleExpressionTimezone = existingSchedule.ScheduleExpressionTimezone,
                    Target = existingSchedule.Target,
                };

                var updateScheduleResponse = await _schedulerClient.UpdateScheduleAsync(updateScheduleRequest);
                return updateScheduleResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error enabling schedule '{scheduleName}': {ex.Message}");
            }
        }
    }
}