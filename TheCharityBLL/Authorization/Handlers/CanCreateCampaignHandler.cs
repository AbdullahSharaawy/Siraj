using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TheCharityBLL.Authorization.Requirements;
using IAuthorizationService = TheCharityBLL.Services.Abstraction.IAuthorizationService;

namespace TheCharityBLL.Authorization.Handlers
{
    public class CanCreateCampaignHandler : AuthorizationHandler<CanCreateCampaignRequirement>
    {
        private readonly IAuthorizationService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CanCreateCampaignHandler(
            IAuthorizationService authService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanCreateCampaignRequirement requirement)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Fail();
                return;
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Fail();
                return;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            // Get organization ID from request body
            var organizationId = GetOrganizationIdFromBody(httpContext);
            if (!organizationId.HasValue)
            {
                context.Fail();
                return;
            }

            var canCreate = await _authService.CanCreateCampaignForOrganizationAsync(userId, organizationId.Value);
            if (canCreate)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }

        private int? GetOrganizationIdFromBody(HttpContext httpContext)
        {
            // Try to get from route (for shared campaigns)
            if (httpContext.Request.RouteValues.TryGetValue("organizationId", out var idObj))
            {
                if (int.TryParse(idObj?.ToString(), out int id))
                    return id;
            }

            // For POST requests, read from body
            // This requires reading the request body - you might want to use a custom model binder
            // For simplicity, you can also get it from the DTO property

            // Alternative: Get from query string
            if (httpContext.Request.Query.TryGetValue("organizationId", out var queryValue))
            {
                if (int.TryParse(queryValue, out int id))
                    return id;
            }

            return null;
        }
    }
}
