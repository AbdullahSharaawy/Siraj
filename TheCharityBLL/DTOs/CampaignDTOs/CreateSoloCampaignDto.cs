namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CreateSoloCampaignDto : CreateCampaignDto
    {
        public int OrganizationId { get; set; }
        public string? ImgPath { get; set; }
        public int? Target { get; set; }
    }
}
