using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagement.Tests.IntegrationTests;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.UserDTOs;
using TheCharityBLL.DTOs.UserResponseDTOs;
using TheCharityDAL.Entities;
using TheCharityBLL.DTOs.UserRequestDTOs;

namespace Charity.Tests.IntegrationTests
{
    public class UserFlowIntegrationTests: IClassFixture<CustomWebApplicationFactory>
    {
        private  readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private  string token = string.Empty;
        private string email = string.Empty;
        private string userName = string.Empty;
        private User user { get; set; }
        private JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        public UserFlowIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

        }
        private async Task ConfirmEmail(string email)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var user = await userManager.FindByEmailAsync(email);
                user.Should().NotBeNull();
                user!.EmailConfirmed = true;
                (await userManager.UpdateAsync(user)).Succeeded.Should().BeTrue();
            }
        }
        private async Task SignUp()
        {
            var postfix = Guid.NewGuid().ToString("N");
            email = $"user{postfix}@gmail.com";
            userName = $"user{postfix}";
            var createdAccountResponse = await _client.PostAsJsonAsync("/api/user/register", new CreateUserRequestDto
            {
                Email = email,
                UserName = userName,
                FullName = "abdullah adel ",
                PhoneNumber = "010275412",
                Address = "giza",
                Password = "123456Ash",
                ConfirmPassword = "123456Ash"
            });
            createdAccountResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdAccountResult = await createdAccountResponse.Content
                .ReadFromJsonAsync<ServiceResponse>(JsonOptions);
            createdAccountResult!.Success.Should().BeTrue();
            
            await ConfirmEmail(email);

        }
        private async Task SignIn()
        {
            var loginResponse = await _client.PostAsJsonAsync("/api/user/login", new LoginRequestDto { Password = "123456Ash", UserName = userName });
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content
                .ReadFromJsonAsync<ServiceResponse<string>>(JsonOptions);
            loginResult!.Success.Should().BeTrue();
            token = loginResult.Data!;
            
            token.Should().NotBeNullOrEmpty();
        }
        private async Task ResetPassword()
        {
            // reset password
            using var scope = _factory.Services.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<User>>();

            var user = await userManager.FindByEmailAsync(email);
            user.Should().NotBeNull();

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user!);

            var resetPasswordResponse = await _client.PostAsJsonAsync(
                "/api/user/reset-password",
                new ResetPasswordResponseDto
                {
                    Email = user!.Email,
                    Password = "123456Ash",
                    Token = resetToken
                });
            resetPasswordResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var resetPasswordResult = await resetPasswordResponse.Content
                .ReadFromJsonAsync<ServiceResponse>(JsonOptions);
            resetPasswordResult!.Success.Should().BeTrue();

        }

        private async Task ChangePassword()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
                "/api/user/change-password",
                new ChangePasswordRequestDto
                {
                    CurrentPassword = "123456Ash",
                    NewPassword = "123456Ash",
                    ConfirmPassword = "123456Ash"
                });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        private async Task assignSuperAdminRole()
        {
            // assing role
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                roleResult.Succeeded.Should().BeTrue();
            }

            user = await userManager.FindByEmailAsync(email);
            user.Should().NotBeNull();

            var addRoleResult = await userManager.AddToRoleAsync(user!, "SuperAdmin");
            addRoleResult.Succeeded.Should().BeTrue();
        }
        [Fact]
        public async Task Flow1_SignUp_SingIn_ResetPassword_ChangePassword()
        {
           
            //sign up
            await SignUp();
            //sign in
           await SignIn();
            // reset and forget password
           await ResetPassword();
            // change password
           await ChangePassword();

        }
        [Fact]
        public async Task Flow2_SignUp_SignIn_GetAll_GetById()
        {
           
            //sign up
            await SignUp();

            await assignSuperAdminRole();
            
            //sign in
            await SignIn();

            // get all
            _client.DefaultRequestHeaders.Authorization =
              new AuthenticationHeaderValue("Bearer", token);

            var getAllUsersResponse = await _client.GetAsync("api/user");
            getAllUsersResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var getAllResult = await getAllUsersResponse.Content
                .ReadFromJsonAsync<ServiceResponse<List<UserListResponseDto>>>(JsonOptions);
            getAllResult.Success.Should().BeTrue();
            // get by id
            var getByIdResponse = await _client.GetAsync($"api/user/{user.Id}");
            getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getByIdResult=await getByIdResponse.Content.ReadFromJsonAsync<ServiceResponse<UserDetailResponseDto>>(JsonOptions);
            getByIdResult.Success.Should().BeTrue();

        }
        [Fact]
        public async Task Flow3_SignUp_SignIn_Update()
        {
            //sign up
            await SignUp();
            //sign in
            await SignIn();

            _client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", token);

            // update
            var updatedResponse = await _client.PutAsJsonAsync("api/user", new EditUserRequestDto { Address = "cairo" });
            updatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedResult=await updatedResponse.Content.ReadFromJsonAsync<ServiceResponse>(JsonOptions);
            updatedResult.Success.Should().BeTrue();
            

        }
      
    }
}
