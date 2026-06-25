using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TheCharityBLL.Authorization.Requirements;
using IAuthorizationService = TheCharityBLL.Services.Abstraction.IAuthorizationService;

namespace TheCharityBLL.Authorization.Handlers
{
    public class IsSuperAdminHandler : AuthorizationHandler<IsSuperAdminRequirement>
    {
        private readonly IAuthorizationService _authService;

        public IsSuperAdminHandler(IAuthorizationService authService)
        {
            _authService = authService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsSuperAdminRequirement requirement)
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

            var isSuperAdmin = await _authService.IsSuperAdminAsync(userId);
            if (isSuperAdmin)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
