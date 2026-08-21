using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.OrganizationRoleDTOs;
using TheCharityBLL.Mapper.OrganizationRoleMapper;
using TheCharityBLL.Services.Abstraction.OrganizationAbstraction;
using TheCharityDAL.Entities;
using TheCharityDAL.FilterModels;
using TheCharityDAL.Repositories.Abstraction;

namespace TheCharityBLL.Services.Implementation.OrganizationImplementation
{
    public class OrganizationRoleService : IOrganizationRoleService
    {
        private readonly IOrganizationRoleRepository _OrganizationRoleRepository;
        public OrganizationRoleService(IOrganizationRoleRepository OrganizationRoleRepository)
        {
            _OrganizationRoleRepository = OrganizationRoleRepository;
        }

        public async Task<ServiceResponse<OrganizationRoleResponseDto>> AddOrganizationRoleAsync(CreateOrganizationRoleDto organizationRole)
        {



            OrganizationRole mappedEntity = new CreateOrganizationRole().MapToOrganizationRole(organizationRole);



            OrganizationRole result = await _OrganizationRoleRepository.AddOrganizationRoleAsync(mappedEntity);

            if (result == null)
            {
                return new ServiceResponse<OrganizationRoleResponseDto>
                {
                    Data = null,
                    Message = "Failed to add the OrganizationRole.",
                    Success = false
                };
            }


            return new ServiceResponse<OrganizationRoleResponseDto>
            {
                Data = new OrganizationRoleResponse().MapToOrganizationRoleResponseDto(result),
                Message = "The OrganizationRole was added successfully.",
                Success = true
            };
        }



        public async Task<ServiceResponse<bool>> DeleteAsync(string userId, int organizationId)
        {
            bool result = await _OrganizationRoleRepository.DeleteAsync(userId, organizationId);

            if (!result)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Failed to delete the OrganizationRole.",
                    Success = false
                };
            }


            return new ServiceResponse<bool>
            {
                Data = true,
                Message = "The OrganizationRole was added successfully.",
                Success = true
            };
        }

        public async Task<ServiceResponse<IEnumerable<OrganizationRoleResponseDto>>> GetAllAsync(QueryParameters query)
        {

            var result = await _OrganizationRoleRepository.GetAllAsync(query);
            IEnumerable<OrganizationRoleResponseDto> item1 = new OrganizationRoleResponse().MapToOrganizationRoleDtoList(result.Item1.ToList());
            if (result.Item2 == 0)
            {
                return new ServiceResponse<IEnumerable<OrganizationRoleResponseDto>>
                {
                    Data = null,
                    Count = result.Item2,
                    Message = "Failed to find the OrganizationRoles.",
                    Success = false
                };
            }
            return new ServiceResponse<IEnumerable<OrganizationRoleResponseDto>>
            {
                Data = item1,
                Count = result.Item2,
                Message = "The OrganizationRoles are founded successfully.",
                Success = true
            };
        }

        public async Task<ServiceResponse<OrganizationRoleResponseDto>> GetByIdAsync(string userId, int organizationId)
        {
            OrganizationRole result = await _OrganizationRoleRepository.GetByIdAsync(userId, organizationId);

            if (result == null)
            {
                return new ServiceResponse<OrganizationRoleResponseDto>
                {
                    Data = null,
                    Message = "Failed to find the OrganizationRole.",
                    Success = false
                };
            }


            return new ServiceResponse<OrganizationRoleResponseDto>
            {
                Data = new OrganizationRoleResponse().MapToOrganizationRoleResponseDto(result),
                Message = "The OrganizationRole is founded successfully.",
                Success = true
            };
        }

        public async Task<ServiceResponse<OrganizationRoleResponseDto>> UpdateAsync(UpdateOrganizationRoleDto OrganizationRole, string userId, int OrganizationId)
        {
            OrganizationRole currentOrganizationRole = await _OrganizationRoleRepository.GetByIdAsync(userId, OrganizationId);
            if (currentOrganizationRole == null)
            {
                return new ServiceResponse<OrganizationRoleResponseDto>
                {
                    Data = null,
                    Message = "OrganizationRole not found.",
                    Success = false
                };
            }
            new UpdateOrganizationRole().MapToOrganizationRole(OrganizationRole, currentOrganizationRole);
            currentOrganizationRole.UpdatedOn = DateTime.UtcNow;

            OrganizationRole result = await _OrganizationRoleRepository.UpdateAsync(currentOrganizationRole);
            if (result == null)
            {
                return new ServiceResponse<OrganizationRoleResponseDto>
                {
                    Data = null,
                    Message = "Failed to update the OrganizationRole.",
                    Success = false
                };
            }


            return new ServiceResponse<OrganizationRoleResponseDto>
            {
                Data = new OrganizationRoleResponse().MapToOrganizationRoleResponseDto(result),
                Message = "The OrganizationRole was updated successfully.",
                Success = true
            };
        }




    }
}
