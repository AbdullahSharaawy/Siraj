using System.ComponentModel.DataAnnotations;
using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    /// <summary>
    /// DTO for updating an existing campaign
    /// </summary>
    public class UpdateCampaignDto
    {
        /// <summary>
        /// Campaign ID (from route)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Updated title (optional)
        /// </summary>
        /// <example>Updated Campaign Title</example>
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters")]
        public string? Title { get; set; }

        /// <summary>
        /// Updated description (optional)
        /// </summary>
        /// <example>Updated description for the campaign</example>
        [MinLength(20, ErrorMessage = "Description must be at least 20 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Updated image path (optional)
        /// </summary>
        /// <example>/images/updated_campaign.jpg</example>
        public string? ImgPath { get; set; }

        /// <summary>
        /// Updated target amount (optional, cannot change for active campaigns)
        /// </summary>
        /// <example>150000</example>
        [Range(1, 10000000, ErrorMessage = "Target must be between 1 and 10,000,000 EGP")]
        public double? Target { get; set; }

        /// <summary>
        /// Updated campaign type (optional)
        /// </summary>
        /// <example>Solo</example>
        public CampaignType? Type { get; set; }
    }
}
