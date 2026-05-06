namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CreateSharedCampaignDto : CreateCampaignDto
    {
        public List<int> OrganizationIds { get; set; } = new List<int>();
    }
}
