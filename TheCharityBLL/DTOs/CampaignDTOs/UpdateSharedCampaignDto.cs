using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class UpdateSharedCampaignDto : UpdateCampaignDto
    {
        public List<int>? OrganizationIds { get; set; }
    }
}
