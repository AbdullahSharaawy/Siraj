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
    public partial class UpdateOrganizationRole
    {
        public partial UpdateOrganizationRoleDto MapToUpdateOrganizationRoleDto(OrganizationRole OrganizationRole);
        public partial void MapToOrganizationRole(UpdateOrganizationRoleDto src,OrganizationRole des);
        public partial List<UpdateOrganizationRoleDto> MapToOrganizationRoleDtoList(List<OrganizationRole> OrganizationRoles);
    }
}
