using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using TheCharityBLL.DTOs;

using TheCharityBLL.Services.Abstraction;


namespace TheCharityPL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalLoginController : ControllerBase
    {
        private IUserService _userService;
        private readonly IConfiguration _configuration;
        public ExternalLoginController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        /// <summary>
        /// login using external provider (Google, Facebook, etc.)
        /// </summary>

        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "ExternalLogin", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }
       
       
        [AllowAnonymous]
        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? "/";

            if (remoteError != null)
                return BadRequest(new ServiceResponse{ Success = false, Message = $"Error from external provider: {remoteError}" });

            // Get the external login info from the authentication cookie
            var authenticateResult = await HttpContext.AuthenticateAsync("ExternalCookie");

            if (!authenticateResult.Succeeded)
                return BadRequest(new ServiceResponse { Message = "Error loading external login information." ,Success=false});

            // Extract provider info
            var externalUser = authenticateResult.Principal;
            var providerKey = externalUser.FindFirstValue(ClaimTypes.NameIdentifier);
            var loginProvider = authenticateResult.Properties.Items[".AuthScheme"];
            var email = externalUser.FindFirstValue(ClaimTypes.Email);

            if (email == null)
                return BadRequest(new ServiceResponse { Success = false, Message = $"Email claim not received from: {loginProvider}" });

            // Check if user exists
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                //  no password needed for external users
                var createResult = await _userService.CreateExternalUserAsync(email);

                if (!createResult.Succeeded)
                    return BadRequest(new ServiceResponse<IEnumerable<string>> { Success = false, Message = "faild to create a new user.", Data = createResult.Errors.Select(e => e.Description) });

                user = await _userService.GetUserByEmailAsync(email);
            }
           
            if (user == null)
                return BadRequest(new ServiceResponse { Success = false, Message = "Failed to retrieve or create user." });

            // Check if external login is linked


            if (! await _userService.IsExternalLoginLinkedAsync(providerKey,loginProvider,user))
            {
                var loginInfo = new UserLoginInfo(loginProvider, providerKey, loginProvider);
               
                await _userService.AddLoginAsync(user, loginInfo);
            }

            // Generate JWT Token
            var token = await _userService.GenerateJwtTokenAsync(user);

            // Sign out of the external cookie
            await HttpContext.SignOutAsync("ExternalCookie");

            // 1. Read the list from appsettings.json
            var allowedFrontends = _configuration.GetSection("AllowedFrontends").Get<List<string>>();

            // 2. Fallback to an empty list if the section is missing to avoid null reference errors
            if (allowedFrontends == null || !allowedFrontends.Any())
            {
                return BadRequest("Frontend configuration is missing.");
            }

            // 3. Validate the returnUrl against the allowed list
            bool isTrustedUrl = allowedFrontends.Any(url => returnUrl.StartsWith(url));

            if (isTrustedUrl)
            {
                return Redirect($"{returnUrl}?token={token}");
            }
            else
            {
                return BadRequest("Invalid return URL");
            }
        }

       
    }
}
