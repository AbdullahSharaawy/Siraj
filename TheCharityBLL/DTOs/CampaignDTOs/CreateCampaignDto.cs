using System.ComponentModel.DataAnnotations;
using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CreateCampaignDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        [MaxLength(1000, ErrorMessage = "Image path cannot exceed 1000 characters.")]
        public string? ImgPath { get; set; }
        [Required(ErrorMessage = "The Target is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Target must be greater than 0.")]
        public int Target { get; set; }

        [EnumDataType(typeof(CampaignStatus), ErrorMessage = "Invalid campaign status.")]
        public CampaignStatus? Status { get; set; } = CampaignStatus.Active;

        [Required(ErrorMessage = "Campaign type is required.")]
        [EnumDataType(typeof(CampaignType), ErrorMessage = "Invalid campaign type.")]
        public CampaignType Type { get; set; }
    }
}
