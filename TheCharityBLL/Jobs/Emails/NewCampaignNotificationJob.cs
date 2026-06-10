using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Context;
using TheCharityBLL.Jobs.Result.Abstraction;
using TheCharityBLL.Jobs.Result.Implementation;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Jobs.Emails
{
    public class NewCampaignNotificationJob : BaseJob
    {
        private readonly ICampaignService _campaignService;
        private readonly IEmailService _emailService;
        private readonly IOrganizationService _organizationService;

        public NewCampaignNotificationJob(
            ICampaignService campaignService,
            IEmailService emailService,
            IOrganizationService organizationService,
            IUserService userService)
        {
            _campaignService = campaignService;
            _emailService = emailService;
            _organizationService = organizationService;
        }

        public override string JobName => "Send new campaign notification";

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
                $"New Campaign Created: {campaign.Data.Title}",
                $"A new campaign '{campaign.Data.Title}' has been created with target ${campaign.Data.Target}. Deadline: {campaign.Data.Deadline:yyyy-MM-dd}"
            );

            return JobResult.Success($"Sent new campaign notification to organization {campaign.Data.OrganizationId}");
        }
    }
}
