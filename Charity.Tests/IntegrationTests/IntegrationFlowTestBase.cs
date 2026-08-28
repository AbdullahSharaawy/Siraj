using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManagement.Tests.IntegrationTests;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.CampaignDTOs;
using TheCharityBLL.DTOs.OrganizationDTOs;
using TheCharityBLL.DTOs.UserRequestDTOs;
using TheCharityDAL.Entities;
using TheCharityDAL.Enums;

namespace Charity.Tests.IntegrationTests
{
    public abstract class IntegrationFlowTestBase : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient Client;
        protected readonly CustomWebApplicationFactory Factory;

        protected readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        protected const string DefaultPassword = "123456Ash";

        protected IntegrationFlowTestBase(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
        }

        protected void SetBearerToken(string token)
        {
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        protected void ClearBearerToken()
        {
            Client.DefaultRequestHeaders.Authorization = null;
        }

        protected async Task ConfirmEmail(string email)
        {
            using var scope = Factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByEmailAsync(email);
            user.Should().NotBeNull();
            user!.EmailConfirmed = true;
            (await userManager.UpdateAsync(user)).Succeeded.Should().BeTrue();
        }

        protected async Task<User> SignUp()
        {
            var postfix = Guid.NewGuid().ToString("N");
            var email = $"user{postfix}@gmail.com";
            var userName = $"user{postfix}";

            var createdAccountResponse = await Client.PostAsJsonAsync("/api/user/register", new CreateUserRequestDto
            {
                Email = email,
                UserName = userName,
                FullName = "abdullah adel",
                PhoneNumber = "010275412",
                Address = "giza",
                Password = DefaultPassword,
                ConfirmPassword = DefaultPassword
            });
            createdAccountResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdAccountResult = await createdAccountResponse.Content
                .ReadFromJsonAsync<ServiceResponse>(JsonOptions);
            createdAccountResult!.Success.Should().BeTrue();

            User user;
            using (var scope = Factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                user = (await userManager.FindByEmailAsync(email))!;
                user.Should().NotBeNull();
            }

            await ConfirmEmail(email);
            return user;
        }

        protected async Task<string> SignIn(User user)
        {
            var loginResponse = await Client.PostAsJsonAsync("/api/user/login", new LoginRequestDto
            {
                Password = DefaultPassword,
                UserName = user.UserName
            });
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content
                .ReadFromJsonAsync<ServiceResponse<string>>(JsonOptions);
            loginResult!.Success.Should().BeTrue();
            loginResult.Data.Should().NotBeNullOrEmpty();
            return loginResult.Data!;
        }

        protected async Task AssignSuperAdminRole(string userId)
        {
            using var scope = Factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                roleResult.Succeeded.Should().BeTrue();
            }

            var user = await userManager.FindByIdAsync(userId);
            user.Should().NotBeNull();

            if (!await userManager.IsInRoleAsync(user!, "SuperAdmin"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(user!, "SuperAdmin");
                addRoleResult.Succeeded.Should().BeTrue();
            }
        }

        protected async Task<(User Admin, string Token)> SignUpAsSuperAdmin()
        {
            var admin = await SignUp();
            await AssignSuperAdminRole(admin.Id);
            var token = await SignIn(admin);
            SetBearerToken(token);
            return (admin, token);
        }

        protected async Task<OrganizationResponseDto> CreateOrganization(string? name = null, string? address = null)
        {
            var response = await Client.PostAsJsonAsync("/api/organization", new CreateOrganizationDto
            {
                Name = name ?? $"Org-{Guid.NewGuid():N}",
                Address = address ?? "Cairo"
            });
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content
                .ReadFromJsonAsync<ServiceResponse<OrganizationResponseDto>>(JsonOptions);
            result!.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            return result.Data!;
        }

        protected async Task<int> CreateSoloCampaign(int organizationId, string? title = null)
        {
            var response = await Client.PostAsJsonAsync("/api/campaign/solo", new CreateSoloCampaignDto
            {
                Title = title ?? $"Campaign-{Guid.NewGuid():N}",
                Description = "Integration test campaign",
                Target = 10000,
                Type = CampaignType.type1,
                StartDate = DateTime.UtcNow.Date,
                Deadline = DateTime.UtcNow.Date.AddDays(30),
                OrganizationId = organizationId
            });
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<int>>(JsonOptions);
            result!.Success.Should().BeTrue();
            return result.Data;
        }

        protected async Task<int> CreateSharedCampaign(int creatorOrganizationId, List<int> organizationIds, string? title = null)
        {
            var response = await Client.PostAsJsonAsync("/api/campaign/shared", new CreateSharedCampaignDto
            {
                Title = title ?? $"Shared-{Guid.NewGuid():N}",
                Description = "Integration test shared campaign",
                Target = 20000,
                Type = CampaignType.type1,
                StartDate = DateTime.UtcNow.Date,
                Deadline = DateTime.UtcNow.Date.AddDays(45),
                CreatorOrganizationId = creatorOrganizationId,
                OrganizationIds = organizationIds
            });
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<int>>(JsonOptions);
            result!.Success.Should().BeTrue();
            return result.Data;
        }
    }
}
