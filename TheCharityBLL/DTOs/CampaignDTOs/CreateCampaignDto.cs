using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CreateCampaignDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double Target { get; set; }
        public CampaignType Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
