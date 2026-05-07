using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    /// <summary>
    /// Base response DTO for campaign information
    /// </summary>
    public class CampaignResponseDto
    {
        /// <summary>
        /// Unique identifier of the campaign
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Campaign title/name
        /// </summary>
        /// <example>Food for 1000 Families</example>
        public string? Title { get; set; }

        /// <summary>
        /// Detailed description of the campaign
        /// </summary>
        /// <example>Providing food packages to underprivileged families during Ramadan</example>
        public string? Description { get; set; }

        /// <summary>
        /// URL or path to campaign image
        /// </summary>
        /// <example>/images/food_campaign.jpg</example>
        public string? ImgPath { get; set; }

        /// <summary>
        /// Target amount to raise (in EGP)
        /// </summary>
        /// <example>100000</example>
        public double? Target { get; set; }

        /// <summary>
        /// Amount raised so far (in EGP)
        /// </summary>
        /// <example>30000</example>
        public double? Achieved { get; set; }

        /// <summary>
        /// Current campaign status
        /// </summary>
        /// <example>Active</example>
        public CampaignStatus? Status { get; set; }

        /// <summary>
        /// Type of campaign (Solo or Shared)
        /// </summary>
        /// <example>Solo</example>
        public CampaignType? Type { get; set; }

        /// <summary>
        /// Whether the campaign is soft-deleted
        /// </summary>
        /// <example>false</example>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Date when the campaign was created
        /// </summary>
        /// <example>2024-02-01T08:00:00</example>
        public DateTime? RegistrationDate { get; set; }

        /// <summary>
        /// Date when the campaign was last updated
        /// </summary>
        /// <example>2024-02-15T14:30:00</example>
        public DateTime? UpdatedOn { get; set; }
    }
}
