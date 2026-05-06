using TheCharityBLL.DTOs.OrganizationDTOs;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class SoloCampaignResponseDto : CampaignResponseDto
    {
        public OrganizationResponseDto? Organization { get; set; }
    }
}
