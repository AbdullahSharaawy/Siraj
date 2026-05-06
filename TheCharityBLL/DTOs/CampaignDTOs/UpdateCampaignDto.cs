namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class UpdateCampaignDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Target { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
