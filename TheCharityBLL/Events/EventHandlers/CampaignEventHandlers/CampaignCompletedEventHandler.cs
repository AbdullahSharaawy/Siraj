using TheCharityBLL.Events.Abstraction;
using TheCharityBLL.Events.CampaignEvents;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Events.EventHandlers.CampaignEventHandlers
{
    public class CampaignCompletedEventHandler : IEventHandler<CampaignCompletedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IOrganizationService _organizationService;

        public CampaignCompletedEventHandler(
            IEmailService emailService,
            IOrganizationService organizationService)
        {
            _emailService = emailService;
            _organizationService = organizationService;
        }

        public async Task HandleAsync(CampaignCompletedEvent @event)
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
                $"🎉 Campaign Completed: {campaign.Title}",
                $"Congratulations! Your campaign '{campaign.Title}' has successfully reached its target of ${campaign.Target}!\n\n" +
                $"Total raised: ${campaign.Achieved}\n" +
                $"Thank you for your efforts!"
            );
        }
    }
}
