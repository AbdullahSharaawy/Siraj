using System.ComponentModel.DataAnnotations;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CreateSharedCampaignDto : CreateCampaignDto
    {
        [MinLength(1, ErrorMessage = "At least one organization must be provided.")]
        public List<int> OrganizationIds { get; set; }
    }
}
