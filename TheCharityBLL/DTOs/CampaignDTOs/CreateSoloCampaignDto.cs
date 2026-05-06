using System.ComponentModel.DataAnnotations;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CreateSoloCampaignDto : CreateCampaignDto
    {
        [Required(ErrorMessage = "OrganizationId is required for a solo campaign.")]
        public int OrganizationId { get; set; }
    }
}
