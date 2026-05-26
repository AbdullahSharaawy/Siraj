namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CampaignDetailsResponseDto : CampaignResponseDto
    {
        /// <example>75.5</example>
        public double AchievementPercentage { get; set; }
        /// <example>25000</example>
        public double RemainingAmount { get; set; }
        /// <example>42</example>
        public int TotalDonationsCount { get; set; }
        public List<DonationBasicDto>? RecentDonations { get; set; }
    }
}
