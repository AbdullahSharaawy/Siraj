using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.DonationDTOs
{
    public class CreateDonationDto
    {
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public double? Amount { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; } = null!;

        [Required(ErrorMessage = "CampaignId is required.")]
        public int? CampaignId { get; set; }
    }
}
