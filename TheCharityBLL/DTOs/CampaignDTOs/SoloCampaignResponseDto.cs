namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class SoloCampaignResponseDto : CampaignResponseDto
    {
        public int? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
    }
}
