using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs.OrganizationRoleDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper.OrganizationRoleMapper
{
    [Mapper]
    public partial class OrganizationRoleResponse
    {
        public partial OrganizationRoleResponseDto MapToOrganizationRoleResponseDto(OrganizationRole OrganizationRole);
        public partial OrganizationRole MapToOrganizationRole(OrganizationRoleResponseDto OrganizationRole);
        public partial List<OrganizationRoleResponseDto> MapToOrganizationRoleDtoList(List<OrganizationRole> OrganizationRoles);
    }
}
