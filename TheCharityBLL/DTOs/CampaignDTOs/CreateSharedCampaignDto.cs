namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CreateSharedCampaignDto : CreateCampaignDto
    {
        public List<int> OrganizationIds { get; set; } = new List<int>();
        public string? ImgPath { get; set; }
        public int? Target { get; set; }
    }
}
