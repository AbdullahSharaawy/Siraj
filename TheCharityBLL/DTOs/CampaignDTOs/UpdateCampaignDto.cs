using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class UpdateCampaignDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImgPath { get; set; }
        public double? Target { get; set; }
        public CampaignType? Type { get; set; }
    }
}
