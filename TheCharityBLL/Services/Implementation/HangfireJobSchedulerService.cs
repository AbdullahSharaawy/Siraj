using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Context;
using TheCharityBLL.Jobs.Result.Implementation;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Services.Implementation
{
    public class HangfireJobSchedulerService : IJobSchedulerService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HangfireJobSchedulerService> _logger;

        public HangfireJobSchedulerService(
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager,
            IServiceProvider serviceProvider,
            ILogger<HangfireJobSchedulerService> logger)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public string EnqueueJob<T>(object? parameters = null) where T : BaseJob
        {
            var jobId = _backgroundJobClient.Enqueue(() => ExecuteJobAsync<T>(parameters, CancellationToken.None));
            _logger.LogInformation("Enqueued job {JobType} with ID: {JobId}", typeof(T).Name, jobId);
            return jobId;
        }

        public string ScheduleJob<T>(DateTimeOffset executeAt, object? parameters = null) where T : BaseJob
        {
            var jobId = _backgroundJobClient.Schedule(() => ExecuteJobAsync<T>(parameters, CancellationToken.None), executeAt);
            _logger.LogInformation("Scheduled job {JobType} at {ExecuteAt} with ID: {JobId}", typeof(T).Name, executeAt, jobId);
            return jobId;
        }

        public void AddOrUpdateRecurringJob<T>(string recurringJobId, string cronExpression, object? parameters = null) where T : BaseJob
        {
            _recurringJobManager.AddOrUpdate(recurringJobId, () => ExecuteJobAsync<T>(parameters, CancellationToken.None), cronExpression);
            _logger.LogInformation("Added recurring job {JobType} with ID: {RecurringJobId}, cron: {Cron}", typeof(T).Name, recurringJobId, cronExpression);
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
            var jobId = _backgroundJobClient.ContinueJobWith(parentJobId, () => ExecuteJobAsync<T>(parameters, CancellationToken.None));
            _logger.LogInformation("Added continuation job {JobType} after parent {ParentJobId} with ID: {JobId}", typeof(T).Name, parentJobId, jobId);
            return jobId;
        }

        // This is the method that Hangfire calls
        public async Task ExecuteJobAsync<T>(object? parameters, CancellationToken cancellationToken) where T : BaseJob
        {
            using var scope = _serviceProvider.CreateScope();

            var job = scope.ServiceProvider.GetRequiredService<T>();
            var context = new JobContext
            {
                JobName = job.JobName,
                ScheduledAt = DateTime.UtcNow
            };

            // Add parameters to context if provided
            if (parameters != null)
            {
                var props = parameters.GetType().GetProperties();
                foreach (var prop in props)
                {
                    context.SetMetadata(prop.Name, prop.GetValue(parameters) ?? "null");
                }
            }

            _logger.LogInformation("Starting job: {JobName}", job.JobName);

            // Check if job can execute
            if (!await job.CanExecuteAsync(context))
            {
                _logger.LogWarning("Job {JobName} cannot execute - validation failed", job.JobName);
                return;
            }

            try
            {
                var result = await job.ExecuteAsync(context);

                if (result.IsSuccess)
                {
                    await job.OnSuccessAsync(context, result);
                    _logger.LogInformation("Job {JobName} completed successfully: {Message}", job.JobName, result.Message);
                }
                else
                {
                    await job.OnFailureAsync(context, result, result.Error ?? new Exception(result.Message));
                    _logger.LogWarning("Job {JobName} failed: {Message}", job.JobName, result.Message);
                }
            }
            catch (Exception ex)
            {
                var failureResult = JobResult.Failure(ex.Message, ex);
                await job.OnFailureAsync(context, failureResult, ex);
                _logger.LogError(ex, "Job {JobName} threw unhandled exception", job.JobName);
                throw;
            }
        }
    }
}
