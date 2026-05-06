using TheCharityBLL.DTOs.OrganizationDTOs;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class SharedCampaignResponseDto : CampaignResponseDto
    {
        public List<OrganizationResponseDto>? Organizations { get; set; }
    }
}
