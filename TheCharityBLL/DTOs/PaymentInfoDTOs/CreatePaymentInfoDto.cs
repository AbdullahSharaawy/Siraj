using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.PaymentInfoDTOs
{
    public class CreatePaymentInfoDto
    {
        [Required(ErrorMessage = "ApiKey is required.")]
        [MaxLength(300, ErrorMessage = "ApiKey cannot exceed 300 characters.")]
        public string ApiKey { get; set; } = null!;

        [Required(ErrorMessage = "IntegrationId is required.")]
        [MaxLength(300, ErrorMessage = "IntegrationId cannot exceed 300 characters.")]
        public string IntegrationId { get; set; } = null!;

        [Required(ErrorMessage = "IframeId is required.")]
        [MaxLength(300, ErrorMessage = "IframeId cannot exceed 300 characters.")]
        public string IframeId { get; set; } = null!;

        [Required(ErrorMessage = "HmacKey is required.")]
        [MaxLength(300, ErrorMessage = "HmacKey cannot exceed 300 characters.")]
        public string HmacKey { get; set; } 

        [Required(ErrorMessage ="The Organization is Required")]
        public int OrganizationId { get; set; }
    }
}
