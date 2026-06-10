using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Context;
using TheCharityBLL.Jobs.Result.Abstraction;
using TheCharityBLL.Jobs.Result.Implementation;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Jobs.Emails
{
    public class CampaignDeadlineReminderJob : BaseJob
    {
        private readonly ICampaignService _campaignService;
        private readonly IEmailService _emailService;
        private readonly IOrganizationService _organizationService;

        public CampaignDeadlineReminderJob(
            ICampaignService campaignService,
            IEmailService emailService,
            IOrganizationService organizationService)
        {
            _campaignService = campaignService;
            _emailService = emailService;
            _organizationService = organizationService;
        }

        public override string JobName => "Send campaign deadline reminders";

        public override async Task<IJobResult> ExecuteAsync(JobContext context)
        {
            var expiringCampaigns = await _campaignService.GetCampaignsExpiringSoonAsync(7);

            if (!expiringCampaigns.Success || expiringCampaigns.Data == null)
                return JobResult.Failure("Failed to get expiring campaigns");

            var remindersSent = 0;

            foreach (var campaign in expiringCampaigns.Data)
            {
                var organization = await _organizationService.GetOrganizationById(campaign.OrganizationId);
                if (!organization.Success) continue;

                // Get organization email from contact methods
                var contactMethods = await _organizationService.GetOrganizationContactMethods(campaign.OrganizationId);
                var emailContact = contactMethods.Data?.FirstOrDefault(cm => cm.Type == ContactType.Email);

                if (emailContact == null || string.IsNullOrEmpty(emailContact.Value)) continue;

                var percentage = campaign.Target > 0 ? (campaign.Achieved / campaign.Target) * 100 : 0;
                var daysLeft = campaign.Deadline.HasValue ? (campaign.Deadline.Value - DateTime.UtcNow).Days : 0;

                await _emailService.SendNotificationAsync(
                    emailContact.Value,
                    $"⚠️ Campaign Ending Soon: {campaign.Title}",
                    $"Your campaign '{campaign.Title}' ends in {daysLeft} days.\n" +
                    $"Current progress: {percentage:F1}% (${campaign.Achieved} / ${campaign.Target})\n" +
                    $"Visit your dashboard to extend the deadline if needed."
                );
                remindersSent++;
            }

            return JobResult.Success($"Sent {remindersSent} deadline reminders");
        }
    }
}
