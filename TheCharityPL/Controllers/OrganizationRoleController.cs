using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheCharityBLL.DTOs.OrganizationRoleDTOs;
using TheCharityBLL.Services.Abstraction.Organization;
using TheCharityDAL.FilterModels;

namespace TheCharityPL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationRoleController : ControllerBase
    {
       
        private readonly IOrganizationRoleService _OrganizationRoleService;
        public OrganizationRoleController( IOrganizationRoleService OrganizationRoleervice)
        {
            
            _OrganizationRoleService = OrganizationRoleervice;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAllOrganizationRole([FromQuery] QueryParameters query)
        {
            var result = await _OrganizationRoleService.GetAllAsync(query);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrganizationRole(CreateOrganizationRoleDto OrganizationRole)
        {
            var result = await _OrganizationRoleService.AddOrganizationRoleAsync(OrganizationRole);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPut("{organizationId}/organizations/{userId}/users")]
        public async Task<IActionResult> UpdateOrganizationRole(UpdateOrganizationRoleDto OrganizationRole,string userId, int organizationId)
        {
            var result = await _OrganizationRoleService.UpdateAsync(OrganizationRole, userId,organizationId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("{organizationId}/organizations/{userId}/users")]
        public async Task<IActionResult> GetOrganizationRole(string userId, int organizationId)
        {
            var result = await _OrganizationRoleService.GetByIdAsync(userId,organizationId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }
        [HttpDelete("{organizationId}/organizations/{userId}/users")]
        public async Task<IActionResult> DeleteOrganizationRole(string userId, int organizationId)
        {
            var result = await _OrganizationRoleService.DeleteAsync(userId, organizationId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }
        
    }
}
