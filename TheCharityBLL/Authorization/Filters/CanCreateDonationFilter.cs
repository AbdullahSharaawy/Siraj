using IAuthorizationService = TheCharityBLL.Services.Abstraction.IAuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TheCharityBLL.DTOs.DonationDTOs;

namespace TheCharityBLL.Authorization.Filters
{
    public class CanCreateDonationFilter : IAsyncActionFilter
    {
         private readonly IAuthorizationService _authService;
         public CanCreateDonationFilter(IAuthorizationService authorizationService)
         {
            _authService=authorizationService;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var authenticatedUserId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var request = context.ActionArguments.Values.OfType<CreateDonationDto>().FirstOrDefault();
            
            if (string.IsNullOrEmpty(authenticatedUserId) || request?.CampaignId is not int campaignId)
            {
                context.Result = new BadRequestObjectResult("CampaignId is required.");
                return;
            }

            if (!await _authService.CanCreateDonationForCampaignAsync(authenticatedUserId, campaignId))
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}