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
    public class UserFlowIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
       
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
        private async Task<User> SignUp()
        {
            User user;
            var postfix = Guid.NewGuid().ToString("N");
            string email = $"user{postfix}@gmail.com";
            string userName = $"user{postfix}";
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

            using (var scope = _factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                user = await userManager.FindByEmailAsync(email);
                user.Should().NotBeNull();
            }
            await ConfirmEmail(email);
            return user;
        }
        private async Task<string> SignIn(User user)
        {
            string token;
            var loginResponse = await _client.PostAsJsonAsync("/api/user/login", new LoginRequestDto { Password = "123456Ash", UserName = user.UserName });
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content
                .ReadFromJsonAsync<ServiceResponse<string>>(JsonOptions);
            loginResult!.Success.Should().BeTrue();
            token = loginResult.Data!;

            token.Should().NotBeNullOrEmpty();
            return token;
        }
        private async Task ResetPassword(User user)
        {
            // reset password
            using var scope = _factory.Services.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<User>>();

            
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

        private async Task ChangePassword(string token)
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
        private async Task assignSuperAdminRole(string userId)
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

            var user = await userManager.FindByIdAsync(userId);
            user.Should().NotBeNull();

            var addRoleResult = await userManager.AddToRoleAsync(user!, "SuperAdmin");
            addRoleResult.Succeeded.Should().BeTrue();
        }
        [Fact]
        public async Task Flow0_SignUp_SignIn_Delete_Restore()
        {
            User userAdmin,guestUser;
            
            string token = string.Empty;
            //sign up
            userAdmin = await SignUp();
            guestUser=await SignUp();
            // assign role
            await assignSuperAdminRole(userAdmin.Id);
            //sign in
            token = await SignIn(userAdmin);
            // delete
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var deletedResponse=await _client.DeleteAsync($"api/user/{guestUser.Id}");
            deletedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deletedResult= await deletedResponse.Content.ReadFromJsonAsync<ServiceResponse>(JsonOptions);
            deletedResult!.Success.Should().BeTrue();

            // restore
            var restoreResponse = await _client.GetAsync($"api/user/restore/{guestUser.Id}");
            restoreResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var restoreResult = await restoreResponse.Content.ReadFromJsonAsync<ServiceResponse>(JsonOptions);
            restoreResult!.Success.Should().BeTrue();

        }
        [Fact]
        public async Task Flow1_SignUp_SingIn_ResetPassword_ChangePassword()
        {
            string token = string.Empty;
            User user;
            //sign up
            user=await SignUp();
            //sign in
            token= await SignIn(user);
            // reset and forget password
            await ResetPassword(user);
            // change password
            await ChangePassword(token);

        }
        [Fact]
        public async Task Flow2_SignUp_SignIn_GetAll_GetById()
        {
            string token = string.Empty;    
            User user;
            //sign up
             user = await SignUp();

            await assignSuperAdminRole(user.Id);

            //sign in
            token=await SignIn(user);

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
            var getByIdResult = await getByIdResponse.Content.ReadFromJsonAsync<ServiceResponse<UserDetailResponseDto>>(JsonOptions);
            getByIdResult.Success.Should().BeTrue();

        }
        [Fact]
        public async Task Flow3_SignUp_SignIn_Update()
        {
            string token = string.Empty;
            User user;
            //sign up
            user=await SignUp();
            //sign in
            token = await SignIn(user);

            _client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", token);

            // update
            var updatedResponse = await _client.PutAsJsonAsync("api/user", new EditUserRequestDto { Address = "cairo" });
            updatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedResult = await updatedResponse.Content.ReadFromJsonAsync<ServiceResponse>(JsonOptions);
            updatedResult.Success.Should().BeTrue();


        }
        [Fact]
        public async Task Flow4_SignUp_SignIn_AssignRole_GetUserRoles_GetAllRoles_RemoveRole()
        {
            string token = string.Empty;
            User user;
            //sign up
            user = await SignUp();

            // assign role
            await assignSuperAdminRole(user.Id);

            //sign in
            token= await SignIn(user);

            _client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", token);


            // get user roles
            var getUserRolesResponse = await _client.GetAsync($"api/user/{user.Id}/roles");
            getUserRolesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getUserRolesResult = await getUserRolesResponse.Content.ReadFromJsonAsync<ServiceResponse<IList<string>>>(JsonOptions);
            getUserRolesResult.Success.Should().BeTrue();
            getUserRolesResult.Data.Should().Contain("SuperAdmin");

            // get all roles
            var getAllRolesResponse = await _client.GetAsync("api/user/roles/all");
            getAllRolesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getAllRolesResult = await getAllRolesResponse.Content.ReadFromJsonAsync<ServiceResponse<List<string>>>(JsonOptions);
            getAllRolesResult.Success.Should().BeTrue();
            getAllRolesResult.Data.Should().Contain("SuperAdmin");

            // remove role
            var removeRoleResponse = await _client.DeleteAsync($"api/user/{user.Id}/roles/SuperAdmin");
            removeRoleResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        }
    }
}
