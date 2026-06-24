namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CreateSoloCampaignDto : CreateCampaignDto
    {
        /// <example>5</example>
        public int OrganizationId { get; set; }
        /// <example>/images/solo_campaign.jpg</example>
        public string? ImgPath { get; set; }
        /// <example>50000</example>
        public int? Target { get; set; }
    }
}
