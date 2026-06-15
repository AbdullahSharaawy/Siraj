using Hangfire;
using Microsoft.Extensions.Logging;
using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Context;
using TheCharityDAL.Enums;
using TheCharityDAL.Repositories.Abstraction;
using TheCharityBLL.Jobs.Result.Abstraction;
using TheCharityBLL.Jobs.Result.Implementation;

namespace TheCharityBLL.Jobs.Scheduled
{
    [Queue("maintenance")]
    public class AutoCompleteCampaignsJob : BaseJob
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ILogger<AutoCompleteCampaignsJob> _logger;

        public AutoCompleteCampaignsJob(
            ICampaignRepository campaignRepository,
            ILogger<AutoCompleteCampaignsJob> logger)
        {
            _campaignRepository = campaignRepository;
            _logger = logger;
        }

        public override string JobName => "Check and update completed campaigns";

        public override async Task<IJobResult> ExecuteAsync(JobContext context)
        {
            try
            {
                // Get all active campaigns
                var campaigns = await _campaignRepository.GetActiveCampaignsAsync();
                var updatedCount = 0;

                foreach (var campaign in campaigns)
                {
                    // Check if achieved >= target
                    if (campaign.Achieved.HasValue && campaign.Target.HasValue && campaign.Achieved >= campaign.Target)
                    {
                        await _campaignRepository.UpdateCampaignStatusAsync(campaign.Id, CampaignStatus.Completed);
                        updatedCount++;
                        _logger.LogInformation("Campaign {CampaignId} ({Title}) marked as completed", campaign.Id, campaign.Title);
                    }
                }

                return JobResult.Success($"Updated {updatedCount} campaigns to Completed status", new { UpdatedCount = updatedCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check campaigns");
                return JobResult.Failure($"Failed to check campaigns: {ex.Message}", ex);
            }
        }
    }
}
