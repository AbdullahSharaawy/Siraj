using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskManagement.Tests.IntegrationTests;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.DonationDTOs;

namespace Charity.Tests.IntegrationTests
{
    public class DonationFlowIntegrationTests : IntegrationFlowTestBase
    {
        public DonationFlowIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Flow0_Create_GetAll_GetById_GetDetails_Update_Delete_Restore()
        {
            var (admin, _) = await SignUpAsSuperAdmin();
            var donor = await SignUp();
            var org = await CreateOrganization();
            var campaignId = await CreateSoloCampaign(org.Id);

            var createResponse = await Client.PostAsJsonAsync("/api/donations", new CreateDonationDto
            {
                Amount = 250,
                UserId = donor.Id,
                CampaignId = campaignId
            });
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createResult = await createResponse.Content
                .ReadFromJsonAsync<ServiceResponse<DonationResponseDto>>(JsonOptions);
            createResult!.Success.Should().BeTrue();
            createResult.Data.Should().NotBeNull();
            var donationId = createResult.Data!.Id;

            (await Client.GetAsync("/api/donations")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/donations/{donationId}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/donations/{donationId}/details")).StatusCode.Should().Be(HttpStatusCode.OK);

            var update = await Client.PutAsJsonAsync($"/api/donations/{donationId}",
                new UpdateDonationDto { Amount = 300, CampaignId = campaignId });
            update.StatusCode.Should().Be(HttpStatusCode.OK);

            (await Client.GetAsync($"/api/donations/by-campaign/{campaignId}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/donations/by-user/{donor.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/donations/recent?days=7")).StatusCode.Should().Be(HttpStatusCode.OK);

            (await Client.DeleteAsync($"/api/donations/{donationId}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/donations/deleted")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.PatchAsync($"/api/donations/{donationId}/restore", null)).StatusCode.Should().Be(HttpStatusCode.OK);

            admin.Should().NotBeNull();
        }

        [Fact]
        public async Task Flow1_Filters_And_Search()
        {
            var donor = await SignUp();
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization();
            var campaignId = await CreateSoloCampaign(org.Id);

            await Client.PostAsJsonAsync("/api/donations", new CreateDonationDto
            {
                Amount = 150,
                UserId = donor.Id,
                CampaignId = campaignId
            });

            (await Client.GetAsync("/api/donations/by-amount-range?min=1&max=1000")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/donations/search?userId={donor.Id}&campaignId={campaignId}"))
                .StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
