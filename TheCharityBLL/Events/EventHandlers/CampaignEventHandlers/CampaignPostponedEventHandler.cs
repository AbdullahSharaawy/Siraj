using TheCharityBLL.Events.Abstraction;
using TheCharityBLL.Events.CampaignEvents;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Events.EventHandlers.CampaignEventHandlers
{
    public class CampaignPostponedEventHandler : IEventHandler<CampaignPostponedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IOrganizationService _organizationService;

        public CampaignPostponedEventHandler(
            IEmailService emailService,
            IOrganizationService organizationService)
        {
            _emailService = emailService;
            _organizationService = organizationService;
        }

        public async Task HandleAsync(CampaignPostponedEvent @event)
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
                $"Campaign Postponed: {campaign.Title}",
                $"Your campaign '{campaign.Title}' has been postponed.\n\n" +
                $"You can resume it whenever you're ready by updating the status."
            );
        }
    }
}
