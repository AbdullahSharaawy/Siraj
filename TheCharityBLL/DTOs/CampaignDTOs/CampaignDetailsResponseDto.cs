using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CampaignDetailsResponseDto : CampaignResponseDto
    {
        public List<DonationBasicDto>? RecentDonations { get; set; }
        public int TotalDonationsCount { get; set; }
        public double RemainingAmount { get; set; }
        public int DaysRemaining { get; set; }
    }
}
