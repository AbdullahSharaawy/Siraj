using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.CampaignDTOs
{
    public class CreateSoloCampaignDto : CreateCampaignDto
    {
        public int OrganizationId { get; set; }
    }
}
