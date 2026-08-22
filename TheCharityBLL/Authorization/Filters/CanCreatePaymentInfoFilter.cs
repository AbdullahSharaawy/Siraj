using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TheCharityBLL.DTOs.PaymentInfoDTOs;
using IAuthorizationService = TheCharityBLL.Services.Abstraction.IAuthorizationService;

namespace TheCharityBLL.Authorization.Filters
{
    public class CanCreatePaymentInfoFilter : IAsyncActionFilter
    {
        private readonly IAuthorizationService _authService;

        public CanCreatePaymentInfoFilter(IAuthorizationService authService)
        {
            _authService = authService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var request = context.ActionArguments.Values.OfType<CreatePaymentInfoDto>().FirstOrDefault();

            if (string.IsNullOrEmpty(userId) || request == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!await _authService.CanUpdatePaymentInfoAsync(userId, request.OrganizationId))
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}