namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CampaignStatisticsDto
    {
        public int TotalCampaigns { get; set; }
        public int ActiveCampaigns { get; set; }
        public int CompletedCampaigns { get; set; }
        public int PendingCampaigns { get; set; }
        public int CancelledCampaigns { get; set; }
        public double TotalMoneyRaised { get; set; }
        public double AverageAchievementPercentage { get; set; }
        public int SoloCampaignsCount { get; set; }
        public int SharedCampaignsCount { get; set; }
        public CampaignResponseDto? MostSuccessfulCampaign { get; set; }
        public CampaignResponseDto? MostDonatedCampaign { get; set; }
        public DateTime StatisticsDate { get; set; }
    }
}
