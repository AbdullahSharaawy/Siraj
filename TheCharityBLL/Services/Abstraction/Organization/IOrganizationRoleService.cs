using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.OrganizationRoleDTOs;
using TheCharityDAL.Entities;
using TheCharityDAL.FilterModels;

namespace TheCharityBLL.Services.Abstraction.Organization
{
    public interface IOrganizationRoleService
    {
        Task<ServiceResponse<OrganizationRoleResponseDto?>> GetByIdAsync(string userId, int OrganizationId);
        Task<ServiceResponse<IEnumerable<OrganizationRoleResponseDto>>> GetAllAsync(QueryParameters query);
        Task<ServiceResponse<OrganizationRoleResponseDto?>> AddOrganizationRoleAsync(CreateOrganizationRoleDto OrganizationRole);
        Task<ServiceResponse<OrganizationRoleResponseDto?>> UpdateAsync(UpdateOrganizationRoleDto OrganizationRole, string userId, int OrganizationId);
        Task<ServiceResponse<bool>> DeleteAsync(string userId, int organizationId);
    }
}
