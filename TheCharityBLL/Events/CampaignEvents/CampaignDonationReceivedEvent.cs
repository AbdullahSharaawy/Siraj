using TheCharityDAL.Entities;

namespace TheCharityBLL.Events.CampaignEvents
{
    public class CampaignDonationReceivedEvent
    {
        public Campaign Campaign { get; set; } = null!;
        public double Amount { get; set; }
    }
}
