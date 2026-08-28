using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskManagement.Tests.IntegrationTests;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.DonatedItemDTOs;
using TheCharityDAL.Enums;

namespace Charity.Tests.IntegrationTests
{
    public class DonatedItemFlowIntegrationTests : IntegrationFlowTestBase
    {
        public DonatedItemFlowIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Flow0_Create_GetAll_GetById_Update_Filters_Delete_Restore()
        {
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization();
            var donor = await SignUp();
            ClearBearerToken();

            var createResponse = await Client.PostAsJsonAsync("/api/donateditem", new CreateDonatedItemDto
            {
                DonorId = donor.Id,
                OrganizationId = org.Id,
                Name = "Winter Jacket",
                Description = "Warm jacket",
                ItemCategory = ItemCategory.Clothes
            });
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createResult = await createResponse.Content
                .ReadFromJsonAsync<ServiceResponse<DonatedItemResponseDto>>(JsonOptions);
            createResult!.Success.Should().BeTrue();
            createResult.Data.Should().NotBeNull();
            var itemId = createResult.Data!.Id;

            (await Client.GetAsync("/api/donateditem")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/donateditem/{itemId}")).StatusCode.Should().Be(HttpStatusCode.OK);

            var update = await Client.PutAsJsonAsync($"/api/donateditem/{itemId}",
                new UpdateDonatedItemDto
                {
                    Name = "Winter Jacket XL",
                    Description = "Updated",
                    ItemCategory = ItemCategory.Clothes,
                    IsAvailable = true
                });
            update.StatusCode.Should().Be(HttpStatusCode.OK);

            (await Client.GetAsync($"/api/donateditem/filter/organization/{org.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/donateditem/filter/donor/{donor.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/donateditem/filter/category?category={ItemCategory.Clothes}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/donateditem/filter/available")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/donateditem/filter/unavailable")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/donateditem/search?searchTerm=Jacket")).StatusCode.Should().Be(HttpStatusCode.OK);

            var delete = await Client.DeleteAsync($"/api/donateditem/{itemId}");
            delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

            (await Client.GetAsync("/api/donateditem/filter/deleted")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.PatchAsync($"/api/donateditem/{itemId}/restore", null)).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Flow1_MarkUnavailable_And_TransferOrganization()
        {
            await SignUpAsSuperAdmin();
            var orgA = await CreateOrganization("Org A");
            var orgB = await CreateOrganization("Org B");
            var donor = await SignUp();
            ClearBearerToken();

            var createResponse = await Client.PostAsJsonAsync("/api/donateditem", new CreateDonatedItemDto
            {
                DonorId = donor.Id,
                OrganizationId = orgA.Id,
                Name = "Laptop",
                ItemCategory = ItemCategory.Electronics
            });
            var createResult = await createResponse.Content
                .ReadFromJsonAsync<ServiceResponse<DonatedItemResponseDto>>(JsonOptions);
            var itemId = createResult!.Data!.Id;

            (await Client.PutAsJsonAsync($"/api/donateditem/{itemId}",
                new UpdateDonatedItemDto { IsAvailable = false })).StatusCode.Should().Be(HttpStatusCode.OK);

            (await Client.PutAsJsonAsync($"/api/donateditem/{itemId}",
                new UpdateDonatedItemDto { OrganizationId = orgB.Id, IsAvailable = true }))
                .StatusCode.Should().Be(HttpStatusCode.OK);

            var getById = await Client.GetAsync($"/api/donateditem/{itemId}");
            var item = await getById.Content.ReadFromJsonAsync<ServiceResponse<DonatedItemResponseDto>>(JsonOptions);
            item!.Data!.OrganizationId.Should().Be(orgB.Id);
        }
    }
}
