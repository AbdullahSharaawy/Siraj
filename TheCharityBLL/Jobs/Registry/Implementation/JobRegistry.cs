using Hangfire;
using Microsoft.Extensions.Logging;
using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Registry.Abstraction;
using TheCharityBLL.Jobs.Scheduled;
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

            // Register CheckExpiredCampaignsJob to run every hour
            // Add more recurring jobs here as you create them
            _jobScheduler.AddOrUpdateRecurringJob<CheckExpiredCampaignsJob>(
                "check-completed-campaigns",
                Cron.Hourly()
            );

            _logger.LogInformation("All recurring jobs registered successfully");
        }
    }
}
