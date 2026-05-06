using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CreateSharedCampaignDto : CreateCampaignDto
    {
        public List<int> OrganizationIds { get; set; } = new List<int>();
    }
}
