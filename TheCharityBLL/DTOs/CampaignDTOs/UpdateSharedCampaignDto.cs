namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class UpdateSharedCampaignDto : UpdateCampaignDto
    {
        /// <example>[1, 2, 4]</example>
        public List<int>? OrganizationIds { get; set; }
    }
}
