using TheCharityBLL.Events.Abstraction;
using TheCharityBLL.Events.CampaignEvents;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Events.EventHandlers.CampaignEventHandlers
{
    public class CampaignDismissedEventHandler : IEventHandler<CampaignDismissedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IOrganizationService _organizationService;

        public CampaignDismissedEventHandler(
            IEmailService emailService,
            IOrganizationService organizationService)
        {
            _emailService = emailService;
            _organizationService = organizationService;
        }

        public async Task HandleAsync(CampaignDismissedEvent @event)
        {
            var campaign = @event.Campaign;

            if (campaign.OrganizationId == null) return;
            var organization = await _organizationService.GetOrganizationById(campaign.OrganizationId.Value);
            if (!organization.Success) return;

            var contactMethods = await _organizationService.GetOrganizationContactMethods(campaign.OrganizationId.Value);
            var emailContact = contactMethods.Data?.FirstOrDefault(cm => cm.Type == ContactType.Email);

            if (emailContact == null || string.IsNullOrEmpty(emailContact.Value)) return;

            await _emailService.SendNotificationAsync(
                emailContact.Value,
                $"Campaign Dismissed: {campaign.Title}",
                $"Your campaign '{campaign.Title}' has been dismissed.\n\n" +
                $"Target: ${campaign.Target}\n" +
                $"Achieved: ${campaign.Achieved}\n\n" +
                $"You can create a new campaign whenever you're ready."
            );
        }
    }
}
