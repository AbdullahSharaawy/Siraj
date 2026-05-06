using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class SharedCampaignResponseDto : CampaignResponseDto
    {
        public List<OrganizationBasicDto>? Organizations { get; set; }
        public int OrganizationsCount { get; set; }
    }
}
