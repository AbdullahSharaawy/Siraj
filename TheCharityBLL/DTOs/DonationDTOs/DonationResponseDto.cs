using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs.CampaignDTOs;

namespace TheCharityBLL.DTOs.DonationDTOs
{
    public class DonationResponseDto
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public long? transactionId { get; set; }
        public string UserId { get; set; } = null!;
        public int CampaignId { get; set; }
        public CampaignResponseDto? Campaign { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

    }
}
