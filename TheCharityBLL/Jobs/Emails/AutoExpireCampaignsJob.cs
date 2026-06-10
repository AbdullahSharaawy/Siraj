using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Context;
using TheCharityBLL.Jobs.Result.Abstraction;
using TheCharityBLL.Jobs.Result.Implementation;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Jobs.Emails
{
    public class AutoExpireCampaignsJob : BaseJob
    {
        private readonly ICampaignService _campaignService;
        private readonly IEmailService _emailService;
        private readonly IOrganizationService _organizationService;

        public AutoExpireCampaignsJob(
            ICampaignService campaignService,
            IEmailService emailService,
            IOrganizationService organizationService)
        {
            _campaignService = campaignService;
            _emailService = emailService;
            _organizationService = organizationService;
        }

        public override string JobName => "Auto-expire overdue campaigns";

        public override async Task<IJobResult> ExecuteAsync(JobContext context)
        {
            var expiredCampaigns = await _campaignService.GetExpiredCampaignsAsync();

            if (!expiredCampaigns.Success || expiredCampaigns.Data == null)
                return JobResult.Failure("Failed to get expired campaigns");

            var expiredCount = 0;

            foreach (var campaign in expiredCampaigns.Data)
            {
                // Only expire active campaigns
                if (campaign.Status == CampaignStatus.Active)
                {
                    await _campaignService.UpdateCampaignStatusAsync(campaign.Id, CampaignStatus.Expired);

                    var organization = await _organizationService.GetOrganizationById(campaign.OrganizationId);
                    if (organization.Success)
                    {
                        // Get organization email
                        var contactMethods = await _organizationService.GetOrganizationContactMethods(campaign.OrganizationId);
                        var emailContact = contactMethods.Data?.FirstOrDefault(cm => cm.Type == ContactType.Email);

                        if (emailContact != null && !string.IsNullOrEmpty(emailContact.Value))
                        {
                            await _emailService.SendNotificationAsync(
                                emailContact.Value,
                                $"Campaign Expired: {campaign.Title}",
                                $"Your campaign '{campaign.Title}' has passed its deadline and has been marked as expired.\n" +
                                $"You can extend the deadline or create a new campaign."
                            );
                        }
                    }
                    expiredCount++;
                }
            }

            return JobResult.Success($"Expired {expiredCount} campaigns");
        }
    }

}
