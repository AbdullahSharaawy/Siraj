namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CampaignValidationDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Target { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? OrganizationId { get; set; }
    }
}
