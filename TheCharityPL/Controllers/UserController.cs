using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.UserDTOs;
using TheCharityBLL.DTOs.UserResponseDTOs;
using TheCharityBLL.Services.Abstraction;


namespace TheCharityPL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
       
        private readonly ILogger<UserController> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserController(
            IUserService userService,
           
            ILogger<UserController> logger,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // ─── Helpers ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads the authenticated user's ID directly from the JWT claim.
        /// No service/DB call needed — replaces the old GetCurrentUserAsync().
        /// </summary>
        private string? GetCurrentUserId()
            => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // ─── GET api/user ────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] bool showDeleted = false)
        {
            try
            {
                _logger.LogInformation("Loading all users");

                var users = await _userService.GetAllUsersAsync();

                if (!showDeleted)
                    users = users.Where(u => !u.IsDeleted);

                var result = users.Select(u => new UserListResponseDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    IsDeleted = u.IsDeleted,
                    RegistrationDate = u.RegistrationDate,
                    EmailConfirmed = u.EmailConfirmed
                }).OrderByDescending(u => u.RegistrationDate).ToList();

                var api_response=new ServiceResponse<List<UserListResponseDto>> 
                {
                    Data = result,
                    Success = true,
                    Message = "Users loaded successfully."
                };
                return Ok(api_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                return StatusCode(500, new ServiceResponse{Success = false, Message = "An error occurred while loading users." });
            }
        }

        // ─── GET api/user/{id} ───────────────────────────────────────────────────────

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                _logger.LogInformation("Loading details for user ID: {UserId}", id);

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return NotFound(new ServiceResponse { Success = false, Message = $"User with ID '{id}' not found." });
                }

                var ResponseDto = new UserDetailResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    IsDeleted = user.IsDeleted,
                    DeletedOn = user.DeletedOn,
                    RegistrationDate = user.RegistrationDate,
                    UpdatedOn = user.UpdatedOn,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    AccessFailedCount = user.AccessFailedCount
                };
                var api_response= new ServiceResponse<UserDetailResponseDto>
                {
                    Data = ResponseDto,
                    Success = true,
                    Message = "User details loaded successfully."
                };
                return Ok(api_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading details for user ID: {UserId}", id);
                return StatusCode(500, new ServiceResponse { Success = false, Message = "An error occurred while loading user details." });
            }
        }

        // ─── POST api/user/register ──────────────────────────────────────────────────

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateUserResponseDto ResponseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ServiceResponse<ModelStateDictionary> { Data = ModelState, Success = false, Message = "your credentials is invalid" });

            try
            {
                _logger.LogInformation("Creating new user with email: {Email}", ResponseDto.Email);

                var existingUsers = await _userService.GetAllUsersAsync();

                if (existingUsers.Any(u => u.Email == ResponseDto.Email))
                    return Conflict(new ServiceResponse{Success = false, Message = "This email is already registered." });

                // FIX: original used return View() which is MVC-only — replaced with proper API response
                if (existingUsers.Any(u => u.UserName == ResponseDto.UserName))
                    return Conflict(new ServiceResponse { Success = false, Message = "This username is already taken." });

                var createUserDTO = new CreateUserDTO
                {
                    Email = ResponseDto.Email,
                    UserName = ResponseDto.UserName,
                    FullName = ResponseDto.FullName,
                    PhoneNumber = ResponseDto.PhoneNumber,
                    Address = ResponseDto.Address,
                    Password = ResponseDto.Password
                };

                var result = await _userService.CreateUserAsync(createUserDTO);

                if (result.Data.Succeeded)
                {
                    var token = await _userService.GenerateEmailConfirmationTokenAsync(createUserDTO.Email);
                    var confirmationLink = BuildFrontendLink("api/User/confirm-email", createUserDTO.Email, token);
                    await _emailService.SendEmailConfirmationAsync(createUserDTO.Email, confirmationLink);

                    return Ok(new ServiceResponse{Success=true, Message = "Registration successful. Please check your email to confirm your account." });
                }

                return BadRequest(new ServiceResponse<IEnumerable< string>>{ Success=false,Message="your credentials is invalid", Data= result.Data.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, new ServiceResponse{Success = false, Message = "An error occurred while creating the user." });
            }
        }

        // ─── POST api/user/login ─────────────────────────────────────────────────────

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginResponseDto ResponseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ServiceResponse<ModelStateDictionary> { Data = ModelState, Success = false, Message = "your credentials is invalid" });


            try
            {
                _logger.LogInformation("Login attempt for: {UserName}", ResponseDto.UserName);

                // Check email confirmed before attempting login
                var user = await _userService.GetUserByEmailAsync(ResponseDto.UserName)
                           ?? (await _userService.GetAllUsersAsync())
                               .FirstOrDefault(u => u.UserName == ResponseDto.UserName && !u.IsDeleted);

                if (user == null)
                    return Unauthorized(new ServiceResponse{Success = false, Message = "Invalid credentials." });

                if (!user.EmailConfirmed)
                    return Unauthorized(new ServiceResponse{Success = false, Message = "Please confirm your email before logging in." });

                // AuthService handles password validation, lockout, and JWT generation
                var token = await _userService.LoginAsync(ResponseDto.UserName, ResponseDto.Password);

                if (token == null)
                {
                    _logger.LogWarning("Login failed for: {UserName}", ResponseDto.UserName);
                    return Unauthorized(new ServiceResponse { Success = false, Message = "Invalid credentials." });
                }

                _logger.LogInformation("User logged in successfully: {UserName}", ResponseDto.UserName);
                return Ok(new ServiceResponse<string>{ Data = token?.Data, Message= $"User logged in successfully: {ResponseDto.UserName}",Success=true});
                }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for: {UserName}", ResponseDto.UserName);
                return StatusCode(500, new ServiceResponse{Success = false, Message = "An error occurred during login." });
            }
        }

        // ─── POST api/user/resend-confirmation ───────────────────────────────────────

        [HttpPost("resend-confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new ServiceResponse{Success = false, Message = "Email is required." });

            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound(new ServiceResponse{Success = false, Message = "User not found." });

                if (user.EmailConfirmed)
                    return BadRequest(new ServiceResponse{Success = false, Message = "Email is already confirmed." });

                var token = await _userService.GenerateEmailConfirmationTokenAsync(email);
                var confirmationLink = BuildFrontendLink("api/User/confirm-email", email, token);
                await _emailService.SendEmailConfirmationAsync(email, confirmationLink);

                return Ok(new ServiceResponse{Success = true, Message = "If the email exists, a confirmation link has been sent." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending confirmation email");
                return StatusCode(500, new ServiceResponse { Success = false, Message = "An error occurred while resending the confirmation email." });
            }
        }

        // ─── POST api/user/confirm-email ─────────────────────────────────────────────

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string encodedToken)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(encodedToken))
                return BadRequest(new ServiceResponse{Success = false, Message = "Email and token are required." });

            try
            {
                var result = await _userService.ConfirmEmailAsync(email, encodedToken);

                if (result.Succeeded)
                    return Ok(new ServiceResponse{Success = true, Message = "Email confirmed successfully." });

                return BadRequest(new ServiceResponse<IEnumerable<string>>{Success=false, Message = "Email confirmation failed.", Data = result.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email");
                return StatusCode(500, new ServiceResponse { Success = false, Message = "An error occurred while confirming the email." });
            }
        }

        // ─── POST api/user/forgot-password ───────────────────────────────────────────

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new ServiceResponse { Success = false, Message = "Email is required." });

            try
            {
                var user = await _userService.GetUserByEmailAsync(email);

                // Always return Ok to avoid revealing whether the email exists
                if (user != null)
                {
                    var token = await _userService.GeneratePasswordResetTokenAsync(user.Id);
                    var resetLink = BuildFrontendLink("reset-password", email, token);
                    await _emailService.SendPasswordResetAsync(email, resetLink);
                }

                return Ok(new ServiceResponse { Success = true, Message = "If your email is registered, you will receive a password reset link shortly." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password request");
                return StatusCode(500, new ServiceResponse { Success = false, Message = "An error occurred while processing your request." });
            }
        }

        // ─── POST api/user/reset-password ────────────────────────────────────────────

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordResponseDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ServiceResponse<ModelStateDictionary> { Data = ModelState, Success = false, Message = "your credentials is invalid" });


            try
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);

                // Return Ok to avoid email enumeration
                if (user == null)
                    return Ok(new ServiceResponse { Success = true, Message = "Password has been reset successfully." });

                var result = await _userService.ResetPasswordAsync(user.Id, model.Token, model.Password);

                if (result.Succeeded)
                {
                    try { await _emailService.SendPasswordChangedNotificationAsync(user.Email); }
                    catch (Exception ex) { _logger.LogError(ex, "Failed to send password change notification"); }

                    return Ok(new ServiceResponse { Success = true, Message = "Password has been reset successfully." });
                }

                return BadRequest(new ServiceResponse<IEnumerable<string>>{Message="your credentials is invalid",Success=false, Data = result.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return StatusCode(500, new ServiceResponse{Success = false, Message = "An error occurred while resetting the password." });
            }
        }

        // ─── PUT api/user/{id} ───────────────────────────────────────────────────────

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] EditUserResponseDto ResponseDto)
        {
            if (id != ResponseDto.Id)
                return BadRequest(new ServiceResponse{Success = false, Message = "ID mismatch." });

            if (!ModelState.IsValid)
                return BadRequest(new ServiceResponse<ModelStateDictionary> { Data = ModelState, Success = false, Message = "your credentials is invalid" });


            try
            {
                _logger.LogInformation("Updating user ID: {UserId}", id);

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new ServiceResponse{Success=false, Message = $"User with ID '{id}' not found." });

                var existingUsers = await _userService.GetAllUsersAsync();

                if (existingUsers.Any(u => u.UserName == ResponseDto.UserName && u.Id != id))
                    return Conflict(new ServiceResponse{Success = false, Message = "This username is already taken." });

                if (existingUsers.Any(u => u.Email == ResponseDto.Email && u.Id != id))
                    return Conflict(new ServiceResponse { Success = false, Message = "This email is already registered." });

                var updateUserDTO = new UpdateUserDTO
                {
                    Id = id,
                    UserName = ResponseDto.UserName,
                    Email = ResponseDto.Email,
                    PhoneNumber = ResponseDto.PhoneNumber,
                    Address = ResponseDto.Address
                };

                var result = await _userService.UpdateUserAsync(updateUserDTO);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User updated successfully with ID: {UserId}", id);
                    return Ok(new ServiceResponse { Success = true, Message = $"User '{ResponseDto.UserName}' updated successfully." });
                }

                return BadRequest(new ServiceResponse<IEnumerable<string>>{Success=false,Message="your credentials is invalid", Data = result.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user ID: {UserId}", id);
                return StatusCode(500, new ServiceResponse { Success = false, Message = "An error occurred while updating the user." });
            }
        }

        // ─── PUT api/user/{id}/change-password ───────────────────────────────────────

        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordResponseDto ResponseDto)
        {
            if (id != ResponseDto.UserId)
                return BadRequest(new ServiceResponse{Success = false, Message = "ID mismatch." });

            if (!ModelState.IsValid)
                return BadRequest(new ServiceResponse<ModelStateDictionary> { Data = ModelState, Success = false, Message = "your credentials is invalid" });


            try
            {
                _logger.LogInformation("Changing password for user ID: {UserId}", id);

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new ServiceResponse { Success = false, Message = $"User with ID '{id}' not found." });

                var changePasswordDTO = new ChangePasswordDTO
                {
                    CurrentPassword = ResponseDto.CurrentPassword,
                    NewPassword = ResponseDto.NewPassword,
                    ConfirmPassword = ResponseDto.ConfirmPassword
                };

                var result = await _userService.ChangeUserPasswordAsync(user.Id, changePasswordDTO);

                if (result.Succeeded)
                {
                    await _emailService.SendPasswordChangedNotificationAsync(user.Email);
                    _logger.LogInformation("Password changed successfully for user ID: {UserId}", id);
                    return Ok(new ServiceResponse{Success = true, Message = "Password changed successfully." });
                }

                return BadRequest(new ServiceResponse<IEnumerable<string>> { Success = false, Message = "your credentials is invalid", Data = result.Errors.Select(e => e.Description) });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user ID: {UserId}", id);
                return StatusCode(500, new ServiceResponse { Success = false, Message = "An error occurred while changing the password." });
            }
        }

        // ─── DELETE api/user/{id} ────────────────────────────────────────────────────

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting user ID: {UserId}", id);

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new ServiceResponse{Success = false, Message = $"User with ID '{id}' not found." });

                var currentUserId = GetCurrentUserId();
                var result = await _userService.DeleteUserAsync(user.Id);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User deleted successfully with ID: {UserId}", id);

                    // FIX: removed LogoutAsync() call — JWT is stateless, client discards the token
                    if (currentUserId == user.Id)
                        return Ok(new ServiceResponse{ Message = "Your account has been deleted. Please discard your token.", Success = true });

                    return Ok(new ServiceResponse { Success = true, Message = $"User '{user.UserName}' deleted successfully." });
                }

                return BadRequest(new ServiceResponse{Success = false, Message = "An error occurred while deleting the user." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user ID: {UserId}", id);
                return StatusCode(500, new ServiceResponse { Success = false, Message = "An error occurred while deleting the user." });
            }
        }

        // ─── POST api/user/restore/{id} ──────────────────────────────────────────────

        [HttpPost("restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Restore(string id)  // FIX: removed [FromBody] — id comes from route
        {
            try
            {
                _logger.LogInformation("Restoring user ID: {UserId}", id);

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new ServiceResponse{Success=false, Message = $"User with ID '{id}' not found." });

                var result = await _userService.RestoreUserAsync(id);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User restored successfully with ID: {UserId}", id);
                    return Ok(new ServiceResponse { Success = true, Message = "User restored successfully." });
                }

                return BadRequest(new ServiceResponse { Success = false, Message = "An error occurred while restoring the user." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring user ID: {UserId}", id);
                return StatusCode(500, new ServiceResponse{Success = false, Message = "An error occurred while restoring the user." });
            }
        }

        // ─── POST api/user/send-notification ─────────────────────────────────────────

        [HttpPost("send-notification")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationResponseDto model)
        {
            try
            {
                await _emailService.SendNotificationAsync(model.Email, model.Subject, model.Message);
                return Ok(new ServiceResponse { Success = true, Message = "Notification sent successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification");
                return StatusCode(500, new ServiceResponse { Success = false, Message = "Failed to send notification. Please try again." });
            }
        }

        // ─── Private Helpers ─────────────────────────────────────────────────────────

        private string BuildFrontendLink(string path, string email, string token)
        {
            var frontendUrl = _configuration["FrontendUrl"];
            var encodedToken = Uri.EscapeDataString(token);
            return $"{frontendUrl}/{path}?email={email}&encodedToken={encodedToken}";
        }
    }
}
