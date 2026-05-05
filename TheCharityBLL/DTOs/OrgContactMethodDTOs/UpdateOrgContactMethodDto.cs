using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.OrganizationContactMethodDTOs
{
    public class UpdateOrgContactMethodDto
    {

        [MaxLength(200, ErrorMessage = "Value cannot exceed 200 characters.")]
        public string? Value { get; set; }

        [EnumDataType(typeof(ContactType), ErrorMessage = "Invalid contact type.")]
        public ContactType? Type { get; set; }

    }
}
