using TheCharityBLL.Events.Abstraction;
using TheCharityBLL.Events.CampaignEvents;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Events.EventHandlers.CampaignEventHandlers
{
    public class CampaignExpiredEventHandler : IEventHandler<CampaignExpiredEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IOrganizationService _organizationService;

        public CampaignExpiredEventHandler(
            IEmailService emailService,
            IOrganizationService organizationService)
        {
            _emailService = emailService;
            _organizationService = organizationService;
        }

        public async Task HandleAsync(CampaignExpiredEvent @event)
        {
            var campaign = @event.Campaign;

            // Get organization details
            if (!campaign.OrganizationId.HasValue) return;
            var organization = await _organizationService.GetOrganizationById(campaign.OrganizationId.Value);
            if (!organization.Success) return;

            // Get organization email from contact methods
            var contactMethods = await _organizationService.GetOrganizationContactMethods(campaign.OrganizationId.Value);
            var emailContact = contactMethods.Data?.FirstOrDefault(cm => cm.Type == ContactType.Email);

            if (emailContact == null || string.IsNullOrEmpty(emailContact.Value)) return;

            // Send email notification
            await _emailService.SendNotificationAsync(
                emailContact.Value,
                $"Campaign Expired: {campaign.Title}",
                $"Your campaign '{campaign.Title}' has passed its deadline and has been marked as expired.\n\n" +
                $"Campaign Details:\n" +
                $"- Target: ${campaign.Target}\n" +
                $"- Achieved: ${campaign.Achieved}\n" +
                $"- Deadline: {campaign.Deadline:yyyy-MM-dd}\n\n" +
                $"You can extend the deadline by visiting your campaign dashboard."
            );
        }
    }
}
