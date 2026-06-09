using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Context;
using TheCharityBLL.Jobs.Result.Implementation;

namespace TheCharityBLL.Jobs.Services
{
    public class JobExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobExecutor> _logger;

        public JobExecutor(IServiceProvider serviceProvider, ILogger<JobExecutor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Executes a job by its type name. This method is called by Hangfire.
        /// </summary>
        /// <param name="jobTypeName">The AssemblyQualifiedName of the job type</param>
        /// <param name="serializedParameters">Optional JSON serialized parameters</param>
        public async Task ExecuteJob(string jobTypeName, string? serializedParameters = null)
        {
            // Get the job type
            var jobType = Type.GetType(jobTypeName);
            if (jobType == null)
            {
                _logger.LogError("Job type not found: {JobTypeName}", jobTypeName);
                return;
            }

            // Create a scope for scoped services (DbContext, Repositories)
            using var scope = _serviceProvider.CreateScope();

            // Resolve the job from DI container
            var job = scope.ServiceProvider.GetRequiredService(jobType) as BaseJob;

            if (job == null)
            {
                _logger.LogError("Job {JobTypeName} is not a valid BaseJob or not registered", jobTypeName);
                return;
            }

            // Create job context
            var context = new JobContext
            {
                JobName = job.JobName,
                ScheduledAt = DateTime.UtcNow,
                JobId = Guid.NewGuid().ToString()
            };

            // Deserialize parameters if provided
            if (!string.IsNullOrEmpty(serializedParameters))
            {
                try
                {
                    var parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(serializedParameters);
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            context.SetMetadata(param.Key, param.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize job parameters for {JobName}", job.JobName);
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
                // Execute the job
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
                _logger.LogError(ex, "Job {JobName} threw an unhandled exception", job.JobName);
                throw;
            }
        }
    }
}
