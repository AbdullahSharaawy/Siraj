using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Context;
using TheCharityBLL.Jobs.Result.Abstraction;
using TheCharityBLL.Jobs.Result.Implementation;
using TheCharityBLL.Services.Abstraction;

namespace TheCharityBLL.Jobs.Emails
{
    public class WeeklyCampaignDigestJob : BaseJob
    {
        private readonly ICampaignService _campaignService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IOrganizationService _organizationService;

        public WeeklyCampaignDigestJob(
            ICampaignService campaignService,
            IEmailService emailService,
            IUserService userService,
            IOrganizationService organizationService)
        {
            _campaignService = campaignService;
            _emailService = emailService;
            _userService = userService;
            _organizationService = organizationService;
        }

        public override string JobName => "Send weekly campaign digest";

        public override async Task<IJobResult> ExecuteAsync(JobContext context)
        {
            var statistics = await _campaignService.GetCampaignStatisticsAsync();
            if (!statistics.Success)
                return JobResult.Failure("Failed to get campaign statistics");

            var activeCount = await _campaignService.GetTotalActiveCampaignsCountAsync();
            var totalRaised = await _campaignService.GetTotalMoneyRaisedAsync();
            var expiringSoon = await _campaignService.GetCampaignsExpiringSoonAsync(7);
            var topCampaigns = await _campaignService.GetTopCampaignsByDonationsAsync(5);

            // TODO: Get all super admins

            var digestMessage = $"📊 Weekly Campaign Digest\n\n" +
                                $"Total Campaigns: {statistics.Data.TotalCampaigns}\n" +
                                $"Active Campaigns: {activeCount.Data}\n" +
                                $"Completed Campaigns: {statistics.Data.CompletedCampaigns}\n" +
                                $"Total Money Raised: ${totalRaised.Data}\n" +
                                $"Average Achievement: {statistics.Data.AverageAchievementPercentage:F1}%\n\n" +
                                $"⚠️ Campaigns expiring within 7 days: {expiringSoon.Data?.Count() ?? 0}\n\n" +
                                $"🏆 Top 5 Campaigns by Donations:\n";

            if (topCampaigns.Data != null)
            {
                var rank = 1;
                foreach (var campaign in topCampaigns.Data.Take(5))
                {
                    digestMessage += $"{rank}. {campaign.Title} - ${campaign.Achieved} / ${campaign.Target}\n";
                    rank++;
                }
            }

            // TODO: Send digestMessage to all super admins

            return JobResult.Success("Weekly campaign digest generated");
        }
    }
}
