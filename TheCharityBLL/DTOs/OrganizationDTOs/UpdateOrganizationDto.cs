using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.OrganizationDTOs
{
    public class UpdateOrganizationDto
    {

        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string? Name { get; set; }

        [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }

    }
}
