using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.OrganizationContactMethodDTOs
{
    public class CreateOrgContactMethodDto
    {
        [Required(ErrorMessage = "Value is required.")]
        [MaxLength(200, ErrorMessage = "Value cannot exceed 200 characters.")]
        public string Value { get; set; } = null!;

        [Required(ErrorMessage = "Contact type is required.")]
        [EnumDataType(typeof(ContactType), ErrorMessage = "Invalid contact type.")]
        public ContactType Type { get; set; }

        [Required(ErrorMessage = "CompanyId is required.")]
        public int CompanyId { get; set; }
    }
}
