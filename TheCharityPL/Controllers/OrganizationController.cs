using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using TheCharityBLL.Authorization.Attributes;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.OrganizationContactMethodDTOs;
using TheCharityBLL.DTOs.OrganizationDTOs;
using TheCharityBLL.DTOs.PaymentInfoDTOs;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Enums;

namespace TheCharityPL.Controllers
{
    [Route("api/[controller]")]//we must specifc roles and policies for each endpoint
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        //organization

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var result = await _organizationService.GetAllOrganizations(includeDeleted);
            return Ok(result);
        }

        [HttpGet("{orgId:int}")]
        public async Task<IActionResult> GetById(int orgId)
        {
            var result = await _organizationService.GetOrganizationById(orgId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{orgId:int}/details")]
        public async Task<IActionResult> GetDetails(int orgId)
        {
            var result = await _organizationService.GetOrganizationDetails(orgId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrganizationDto dto)
        {
            var result = await _organizationService.CreateOrganization(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { orgId = result.Data.Id }, result);
        }

        [HttpPut("{orgId:int}")]
        public async Task<IActionResult> Update(int orgId, [FromBody] UpdateOrganizationDto dto)
        {
            var result = await _organizationService.UpdateOrganization(orgId,dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{orgId:int}")]
        public async Task<IActionResult> Delete(int orgId)
        {
            var result = await _organizationService.DeleteOrganization(orgId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPatch("{orgId:int}/restore")]
        public async Task<IActionResult> Restore(int orgId)
        {
            var result = await _organizationService.RestoreOrganization(orgId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeleted()
        {
            var result = await _organizationService.GetDeletedOrganizations();
            return Ok(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var result = await _organizationService.GetOrganizationsDropDown();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var result = await _organizationService.SearchOrganizations(term);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("filter/by-name")]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            var result = await _organizationService.GetOrganizationByName(name);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("filter/by-address")]
        public async Task<IActionResult> GetByAddress([FromQuery] string address)
        {
            var result = await _organizationService.GetOrganizationsByAddress(address);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int days)
        {
            var result = await _organizationService.GetRecentlyRegisteredOrganizations(days);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("count/total")]
        public async Task<IActionResult> GetTotalCount()
        {
            var result = await _organizationService.GetTotalOrganizationsCount();
            return Ok(result);
        }

        [HttpGet("count/active")]
        public async Task<IActionResult> GetActiveCount()
        {
            var result = await _organizationService.GetActiveOrganizationsCount();
            return Ok(result);
        }

        //contact methods

        [HttpGet("{orgId}/contact-methods")]
        public async Task<IActionResult> GetOrganizationContactMethods(int orgId)
        {
            var result = await _organizationService.GetOrganizationContactMethods(orgId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("contact-methods/{contactId}")]
        public async Task<IActionResult> GetContactMethodById(int contactId)
        {
            var result = await _organizationService.GetContactMethodById(contactId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("contact-methods")]
        public async Task<IActionResult> CreateContactMethod(CreateOrgContactMethodDto dto)
        {
            var result = await _organizationService.CreateContactMethod(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("contact-methods/{contactId:int}")]
        public async Task<IActionResult> UpdateContactMethod(int contactId, UpdateOrgContactMethodDto dto)
        {
            var result = await _organizationService.UpdateContactMethod(contactId,dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("contact-methods/{contactId}")]
        public async Task<IActionResult> DeleteContactMethod(int contactId)
        {
            var result = await _organizationService.DeleteContactMethod(contactId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("contact-methods/restore/{contactId}")]
        public async Task<IActionResult> RestoreContactMethod(int contactId)
        {
            var result = await _organizationService.RestoreContactMethod(contactId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{orgId:int}/contact-type")]
        public async Task<IActionResult> GetContactMethodsByType(int orgId, ContactType type)
        {
            var result = await _organizationService.GetContactMethodsByType(orgId, type);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{orgId:int}/contact-type/count")]
        public async Task<IActionResult> GetContactMethodCountByType(int orgId, ContactType type)
        {
            var result = await _organizationService.GetContactMethodCountByType(orgId, type);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("contact-type/{type}")]
        public async Task<IActionResult> GetOrganizationsByContactType(ContactType type)
        {
            var result = await _organizationService.GetOrganizationsByContactType(type);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        //payment

        //[HttpGet("{orgId}/payment")]
        //public async Task<IActionResult> GetPaymentByOrganization(int orgId)
        //{
        //    var result = await _organizationService.GetPaymentInfoByOrganizationId(orgId);
        //    return result.Success ? Ok(result) : NotFound(result);
        //}

        //[HttpGet("payment/{paymentId}")]
        //public async Task<IActionResult> GetPaymentById(int paymentId)
        //{
        //    var result = await _organizationService.GetPaymentInfoById(paymentId);
        //    return result.Success ? Ok(result) : NotFound(result);
        //}

        //[HttpPost("payment")]
        //public async Task<IActionResult> CreatePayment(CreatePaymentInfoDto dto)
        //{
        //    var result = await _organizationService.CreatePaymentInfo(dto);
        //    return result.Success ? Ok(result) : BadRequest(result);
        //}

        //[HttpPut("{paymentId:int}/payment")]
        //public async Task<IActionResult> UpdatePayment(int paymentId, UpdatePaymentInfoDto dto)
        //{
        //    var result = await _organizationService.UpdatePaymentInfo(paymentId,dto);
        //    return result.Success ? Ok(result) : BadRequest(result);
        //}

        //[HttpDelete("payment/{paymentId}")]
        //public async Task<IActionResult> DeletePayment(int paymentId)
        //{
        //    var result = await _organizationService.DeletePaymentInfo(paymentId);
        //    return result.Success ? Ok(result) : NotFound(result);
        //}

        //[HttpPost("payment/restore/{paymentId}")]
        //public async Task<IActionResult> RestorePayment(int paymentId)
        //{
        //    var result = await _organizationService.RestorePaymentInfo(paymentId);
        //    return result.Success ? Ok(result) : NotFound(result);
        //}

        [HttpGet("payment/none")]
        public async Task<IActionResult> GetOrganizationsWithoutPaymentInfo()
        {
            var result = await _organizationService.GetOrganizationsWithoutPaymentInfo();
            return Ok(result);
        }

        [HttpGet("payment/valid")]
        public async Task<IActionResult> GetOrganizationsWithValidPaymentInfo()
        {
            var result = await _organizationService.GetOrganizationsWithValidPaymentInfo();
            return Ok(result);
        }

        [HttpGet("payment/last-update")]
        public async Task<IActionResult> GetLastPaymentUpdates()
        {
            var result = await _organizationService.GetOrganizationLastPaymentUpdate();
            return Ok(result);
        }

        //camaign

        [HttpGet("campaigns/min-count")]
        public async Task<IActionResult> GetOrganizationsByCampaignCount(int minCampaigns)
        {
            var result = await _organizationService.GetOrganizationsByCampaignCount(minCampaigns);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("campaigns/active")]
        public async Task<IActionResult> GetOrganizationsWithActiveCampaigns()
        {
            var result = await _organizationService.GetOrganizationsWithActiveCampaigns();
            return Ok(result);
        }

        [HttpGet("campaigns/completed")]
        public async Task<IActionResult> GetOrganizationsWithCompletedCampaigns()
        {
            var result = await _organizationService.GetOrganizationsWithCompletedCampaigns();
            return Ok(result);
        }

        [HttpGet("campaigns/none")]
        public async Task<IActionResult> GetOrganizationsWithoutCampaigns()
        {
            var result = await _organizationService.GetOrganizationsWithoutCampaigns();
            return Ok(result);
        }

        // ==============================
        // Organization Admin Management
        // ==============================

        /// <summary>
        /// Assign an organization admin (SuperAdmin only)
        /// </summary>
        [HttpPost("{orgId}/admin")]
        [IsSuperAdmin] // Only SuperAdmin can assign organization admin
        public async Task<IActionResult> AssignOrganizationAdmin(int orgId, [FromBody] AssignAdminRequest request)
        {
            var result = await _organizationService.AssignOrganizationAdminAsync(orgId, request.UserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Remove organization admin (SuperAdmin only)
        /// </summary>
        [HttpDelete("{orgId}/admin")]
        [IsSuperAdmin]
        public async Task<IActionResult> RemoveOrganizationAdmin(int orgId)
        {
            var result = await _organizationService.RemoveOrganizationAdminAsync(orgId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Transfer organization admin to another user (SuperAdmin only)
        /// </summary>
        [HttpPost("{orgId}/admin/transfer")]
        [IsSuperAdmin]
        public async Task<IActionResult> TransferOrganizationAdmin(int orgId, [FromBody] AssignAdminRequest request)
        {
            var result = await _organizationService.TransferOrganizationAdminAsync(orgId, request.UserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get organization admin
        /// </summary>
        [HttpGet("{orgId}/admin")]
        [CanManageOrganization] // OrganizationAdmin + SuperAdmin can view
        public async Task<IActionResult> GetOrganizationAdmin(int orgId)
        {
            var result = await _organizationService.GetOrganizationAdminAsync(orgId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // ==============================
        // Sub-Admin Management
        // ==============================

        /// <summary>
        /// Add a sub-admin to an organization (SuperAdmin + OrganizationAdmin only)
        /// </summary>
        [HttpPost("{orgId}/sub-admins")]
        [CanManageSubAdmins] // SuperAdmin + OrganizationAdmin (NOT SubAdmin)
        public async Task<IActionResult> AddSubAdmin(int orgId, [FromBody] AssignAdminRequest request)
        {
            var result = await _organizationService.AddSubAdminAsync(orgId, request.UserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Remove a sub-admin from an organization (SuperAdmin + OrganizationAdmin only)
        /// </summary>
        [HttpDelete("{orgId}/sub-admins/{userId}")]
        [CanManageSubAdmins] // SuperAdmin + OrganizationAdmin (NOT SubAdmin)
        public async Task<IActionResult> RemoveSubAdmin(int orgId, string userId)
        {
            var result = await _organizationService.RemoveSubAdminAsync(orgId, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get all sub-admins of an organization
        /// </summary>
        [HttpGet("{orgId}/sub-admins")]
        [CanManageOrganization] // SuperAdmin + OrganizationAdmin + SubAdmin can view
        public async Task<IActionResult> GetSubAdmins(int orgId)
        {
            var result = await _organizationService.GetOrganizationSubAdminsAsync(orgId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Check if a user is a sub-admin of an organization
        /// </summary>
        [HttpGet("{orgId}/sub-admins/{userId}/check")]
        [CanManageOrganization] // SuperAdmin + OrganizationAdmin + SubAdmin
        public async Task<IActionResult> IsUserSubAdmin(int orgId, string userId)
        {
            var result = await _organizationService.IsUserSubAdminAsync(orgId, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
