using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Context;
using TheCharityBLL.Jobs.Result.Abstraction;
using TheCharityBLL.Jobs.Result.Implementation;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Jobs.Emails
{
    public class DeadlineExtensionConfirmationJob : BaseJob
    {
        private readonly ICampaignService _campaignService;
        private readonly IEmailService _emailService;
        private readonly IOrganizationService _organizationService;

        public DeadlineExtensionConfirmationJob(
            ICampaignService campaignService,
            IEmailService emailService,
            IOrganizationService organizationService)
        {
            _campaignService = campaignService;
            _emailService = emailService;
            _organizationService = organizationService;
        }

        public override string JobName => "Send deadline extension confirmation";

        public override async Task<IJobResult> ExecuteAsync(JobContext context)
        {
            var campaignId = context.GetMetadata<int>("CampaignId");

            var campaign = await _campaignService.GetCampaignByIdAsync(campaignId);
            if (!campaign.Success)
                return JobResult.Failure($"Campaign {campaignId} not found");

            var organization = await _organizationService.GetOrganizationById(campaign.Data.OrganizationId);
            if (!organization.Success)
                return JobResult.Failure($"Organization {campaign.Data.OrganizationId} not found");

            // Get organization email from contact methods
            var contactMethods = await _organizationService.GetOrganizationContactMethods(campaign.Data.OrganizationId);
            var emailContact = contactMethods.Data?.FirstOrDefault(cm => cm.Type == ContactType.Email);

            if (emailContact == null || string.IsNullOrEmpty(emailContact.Value))
                return JobResult.Failure($"No email found for organization {campaign.Data.OrganizationId}");

            await _emailService.SendNotificationAsync(
                emailContact.Value,
                $"Campaign Deadline Extended: {campaign.Data.Title}",
                $"The deadline for '{campaign.Data.Title}' has been extended to {campaign.Data.Deadline:yyyy-MM-dd}."
            );

            return JobResult.Success($"Sent deadline extension confirmation for campaign {campaignId}");
        }
    }
}
