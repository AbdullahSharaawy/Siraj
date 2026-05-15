namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class SoloCampaignResponseDto : CampaignResponseDto
    {
        /// <example>5</example>
        public int? OrganizationId { get; set; }
        /// <example>Helping Hands NGO</example>
        public string? OrganizationName { get; set; }
    }
}
