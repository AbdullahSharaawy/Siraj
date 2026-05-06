namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class UpdateSharedCampaignDto : UpdateCampaignDto
    {
        public List<int>? OrganizationIds { get; set; }
    }
}
