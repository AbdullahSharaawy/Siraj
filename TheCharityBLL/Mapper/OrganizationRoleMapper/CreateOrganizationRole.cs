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
    public partial class CreateOrganizationRole
    {
        public partial CreateOrganizationRoleDto MapToCreateOrganizationRoleDto(OrganizationRole OrganizationRole);
        public partial OrganizationRole MapToOrganizationRole(CreateOrganizationRoleDto OrganizationRole);
        public partial List<CreateOrganizationRoleDto> MapToOrganizationRolesDtoList(List<OrganizationRole> OrganizationRoles);
    }
}
