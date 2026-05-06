using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CampaignResponseDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Target { get; set; }
        public double? Achieved { get; set; }
        public CampaignStatus Status { get; set; }
        public CampaignType Type { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double AchievementPercentage { get; set; }
        public bool IsDeleted { get; set; }
    }
}
