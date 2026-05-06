using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class UpdateCampaignMoneyDto
    {
        public int CampaignId { get; set; }
        public double Amount { get; set; }
    }
}
