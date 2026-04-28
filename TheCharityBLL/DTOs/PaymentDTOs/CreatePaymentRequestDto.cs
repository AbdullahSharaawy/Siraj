using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.PaymentDTOs
{
    public class CreatePaymentRequestDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "CampaignId is required.")]
        public int CampaignId { get; set; }

        [Required(ErrorMessage ="OrganizationId is required.")]
        public int OrganizationId { get; set; }
    }
}
