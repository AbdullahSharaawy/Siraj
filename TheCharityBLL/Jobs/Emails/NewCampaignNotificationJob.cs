using TheCharityBLL.Jobs.Base;
using TheCharityBLL.Jobs.Context;
using TheCharityBLL.Jobs.Result.Abstraction;
using TheCharityBLL.Jobs.Result.Implementation;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;
using TheCharityDAL.Repositories.Abstraction;

namespace TheCharityBLL.Jobs.Emails
{
    public class NewCampaignNotificationJob : BaseJob
    {
        private readonly ICampaignService _campaignService;
        private readonly IEmailService _emailService;
        private readonly IOrganizationRepository _organizationRepository;

        public NewCampaignNotificationJob(
            ICampaignService campaignService,
            IEmailService emailService,
            IUserRepository userRepository,
            IOrganizationRepository organizationRepository)
        {
            _campaignService = campaignService;
            _emailService = emailService;
            _organizationRepository = organizationRepository;
        }

        public override string JobName => "Send new campaign notification";

        public override async Task<IJobResult> ExecuteAsync(JobContext context)
        {
            var campaignId = context.GetMetadata<int>("CampaignId");
            var campaign = await _campaignService.GetCampaignByIdAsync(campaignId);

            if (!campaign.Success) return JobResult.Failure($"Campaign {campaignId} not found");
            if (campaign.Data == null) return JobResult.Failure($"Campaign {campaignId} data is null");

            var organization = await _organizationRepository.GetOrganizationByIdAsync(campaign.Data.OrganizationId);

            var emailResult = organization?.ContactMethods.FirstOrDefault(cm => cm.Type == ContactType.Email);
            if (emailResult == null || String.IsNullOrEmpty(emailResult.Value)) return JobResult.Failure($"Organization {campaign.Data.OrganizationId} has no email contact method");

            await _emailService.SendNotificationAsync(
                emailResult.Value,
                $"New Campaign: {campaign.Data.Title}",
                $"A new campaign has been created with target ${campaign.Data.Target}."
            );

            return JobResult.Success($"notification Sent");
        }
    }
}
