namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CampaignStatisticsDto
    {
        /// <example>10</example>
        public int TotalCampaigns { get; set; }
        /// <example>5</example>
        public int ActiveCampaigns { get; set; }
        /// <example>3</example>
        public int CompletedCampaigns { get; set; }
        /// <example>1</example>
        public int PendingCampaigns { get; set; }
        /// <example>1</example>
        public int CancelledCampaigns { get; set; }
        /// <example>150000</example>
        public double TotalMoneyRaised { get; set; }
        /// <example>65.5</example>
        public double AverageAchievementPercentage { get; set; }
        /// <example>4</example>
        public int SoloCampaignsCount { get; set; }
        /// <example>6</example>
        public int SharedCampaignsCount { get; set; }
        public CampaignResponseDto? MostSuccessfulCampaign { get; set; }
        public CampaignResponseDto? MostDonatedCampaign { get; set; }
        /// <example>2024-02-20T12:00:00</example>
        public DateTime StatisticsDate { get; set; }
    }
}
