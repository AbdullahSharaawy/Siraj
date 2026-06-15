namespace TheCharityBLL.Events.CampaignEvents
{
    public class CampaignDonationReceivedEvent
    {
        public int CampaignId { get; set; }
        public double Amount { get; set; }
    }
}
