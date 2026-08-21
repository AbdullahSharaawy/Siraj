using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.OrganizationRoleDTOs
{
    public class UpdateOrganizationRoleDto
    {
        public int OrganizationId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public OrganizationRoleType Role { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserFullName { get; set; }
    }
}
