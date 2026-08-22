using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TheCharityBLL.Authorization.Requirements;
using IAuthorizationService = TheCharityBLL.Services.Abstraction.IAuthorizationService;

namespace TheCharityBLL.Authorization.Handlers
{
    public class CanManageDonationHandler : AuthorizationHandler<CanManageDonationRequirement>
    {
        private readonly IAuthorizationService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CanManageDonationHandler(
            IAuthorizationService authService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanManageDonationRequirement requirement)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                context.Fail();
                return;
            }

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var httpContext = _httpContextAccessor.HttpContext;
            if (string.IsNullOrEmpty(userId) || httpContext == null ||
                !httpContext.Request.RouteValues.TryGetValue("id", out var donationIdValue) ||
                !int.TryParse(donationIdValue?.ToString(), out var donationId))
            {
                context.Fail();
                return;
            }

            if (await _authService.CanManageDonationAsync(userId, donationId))
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}