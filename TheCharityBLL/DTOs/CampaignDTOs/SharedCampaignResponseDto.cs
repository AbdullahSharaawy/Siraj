namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class SharedCampaignResponseDto : CampaignResponseDto
    {
        public List<OrganizationBasicDto>? Organizations { get; set; }
        public int OrganizationsCount { get; set; }
    }
}
