using Microsoft.Extensions.Logging;
using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Registry.Abstraction;
using TheCharityBLL.Services.Abstraction;

namespace TheCharityBLL.Jobs.Registry.Implementation
{
    public class JobRegistry : IJobRegistry
    {
        private readonly IJobSchedulerService _jobScheduler;
        private readonly ILogger<JobRegistry> _logger;

        public JobRegistry(IJobSchedulerService jobScheduler, ILogger<JobRegistry> logger)
        {
            _jobScheduler = jobScheduler;
            _logger = logger;
        }

        public void RegisterAllRecurringJobs()
        {
            _logger.LogInformation("Registering all recurring jobs...");

            // Future jobs will be added here
            // Example: 
            // RegisterRecurringJob<CheckExpiredCampaignsJob>("check-expired-campaigns", Cron.Daily(2));

            _logger.LogInformation("All recurring jobs registered");
        }

        public void RegisterRecurringJob<T>(string jobId, string cronExpression) where T : BaseJob
        {
            _jobScheduler.AddOrUpdateRecurringJob<T>(jobId, cronExpression);
            _logger.LogInformation("Registered recurring job: {JobId}", jobId);
        }
    }
}
