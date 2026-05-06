namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CampaignDetailsResponseDto : CampaignResponseDto
    {
        public double AchievementPercentage { get; set; }
        public double RemainingAmount { get; set; }
        public int TotalDonationsCount { get; set; }
        public List<DonationBasicDto>? RecentDonations { get; set; }
    }
}
