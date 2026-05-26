using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Context;
using TheCharityBLL.Jobs.Result.Implementation;
using TheCharityBLL.Jobs.Services;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Services.Implementation
{
    public class HangfireJobSchedulerService : IJobSchedulerService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly JobExecutor _jobExecutor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HangfireJobSchedulerService> _logger;

        public HangfireJobSchedulerService(
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager,
            JobExecutor jobExecutor,
            IServiceProvider serviceProvider,
            ILogger<HangfireJobSchedulerService> logger)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
            _jobExecutor = jobExecutor;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // Generic method for enqueuing jobs (works fine)
        public string EnqueueJob<T>(object? parameters = null) where T : BaseJob
        {
            var jobTypeName = typeof(T).AssemblyQualifiedName;
            var serializedParams = SerializeParameters(parameters);

            var jobId = _backgroundJobClient.Enqueue(() => _jobExecutor.ExecuteJob(jobTypeName!, serializedParams));
            _logger.LogInformation("Enqueued job {JobType} with ID: {JobId}", typeof(T).Name, jobId);
            return jobId;
        }

        // Generic method for scheduling jobs (works fine)
        public string ScheduleJob<T>(DateTimeOffset executeAt, object? parameters = null) where T : BaseJob
        {
            var jobTypeName = typeof(T).AssemblyQualifiedName;
            var serializedParams = SerializeParameters(parameters);

            var jobId = _backgroundJobClient.Schedule(() => _jobExecutor.ExecuteJob(jobTypeName!, serializedParams), executeAt);
            _logger.LogInformation("Scheduled job {JobType} at {ExecuteAt} with ID: {JobId}", typeof(T).Name, executeAt, jobId);
            return jobId;
        }

        // Recurring jobs - uses JobExecutor pattern
        public void AddOrUpdateRecurringJob<T>(string recurringJobId, string cronExpression) where T : BaseJob
        {
            var jobTypeName = typeof(T).AssemblyQualifiedName;

            _recurringJobManager.AddOrUpdate(
                recurringJobId,
                () => _jobExecutor.ExecuteJob(jobTypeName!, null),
                cronExpression
            );

            _logger.LogInformation("Added recurring job {JobType} with ID: {RecurringJobId}, cron: {Cron}",
                typeof(T).Name, recurringJobId, cronExpression);
        }

        public void RemoveRecurringJob(string recurringJobId)
        {
            _recurringJobManager.RemoveIfExists(recurringJobId);
            _logger.LogInformation("Removed recurring job: {RecurringJobId}", recurringJobId);
        }

        public bool CancelJob(string jobId)
        {
            var deleted = _backgroundJobClient.Delete(jobId);
            if (deleted)
                _logger.LogInformation("Cancelled job: {JobId}", jobId);
            else
                _logger.LogWarning("Failed to cancel job: {JobId}", jobId);
            return deleted;
        }

        public JobStatus GetJobStatus(string jobId)
        {
            var storage = JobStorage.Current;
            var monitoringApi = storage.GetMonitoringApi();
            var jobDetails = monitoringApi.JobDetails(jobId);

            if (jobDetails == null)
                return JobStatus.Pending;

            var history = jobDetails.History;
            if (history.Any())
            {
                var lastState = history.Last().StateName;
                return lastState switch
                {
                    "Succeeded" => JobStatus.Completed,
                    "Failed" => JobStatus.Failed,
                    "Deleted" => JobStatus.Cancelled,
                    "Processing" => JobStatus.Processing,
                    _ => JobStatus.Pending
                };
            }

            return JobStatus.Pending;
        }

        public string ScheduleContinuationJob<T>(string parentJobId, object? parameters = null) where T : BaseJob
        {
            var jobTypeName = typeof(T).AssemblyQualifiedName;
            var serializedParams = SerializeParameters(parameters);

            var jobId = _backgroundJobClient.ContinueJobWith(parentJobId,
                () => _jobExecutor.ExecuteJob(jobTypeName!, serializedParams));

            _logger.LogInformation("Added continuation job {JobType} after parent {ParentJobId} with ID: {JobId}",
                typeof(T).Name, parentJobId, jobId);
            return jobId;
        }

        // Helper method to serialize parameters
        private string? SerializeParameters(object? parameters)
        {
            if (parameters == null) return null;

            try
            {
                return System.Text.Json.JsonSerializer.Serialize(parameters);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to serialize job parameters");
                return null;
            }
        }
    }
}
