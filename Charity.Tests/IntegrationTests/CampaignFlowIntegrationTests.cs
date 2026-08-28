using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskManagement.Tests.IntegrationTests;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.CampaignDTOs;
using TheCharityDAL.Enums;

namespace Charity.Tests.IntegrationTests
{
    public class CampaignFlowIntegrationTests : IntegrationFlowTestBase
    {
        public CampaignFlowIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Flow0_SoloCampaign_Create_Get_Update_Extend_Status_Money_Delete_Restore()
        {
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization();
            var campaignId = await CreateSoloCampaign(org.Id, "Solo Help");

            ClearBearerToken();
            (await Client.GetAsync("/api/campaign")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/campaign/{campaignId}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/campaign/{campaignId}/details")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/campaign/solo/{campaignId}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/campaign/solo/by-organization/{org.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);

            await SignUpAsSuperAdmin();

            var updateSolo = await Client.PutAsJsonAsync($"/api/Campaign/solo/{campaignId}",
                new UpdateSoloCampaignDto
                {
                    Id = campaignId,
                    Title = "Solo Help Updated",
                    Description = "Updated description",
                    Target = 10000,
                    OrganizationId = org.Id
                });
            updateSolo.StatusCode.Should().Be(HttpStatusCode.OK);

            var newDeadline = DateTime.UtcNow.Date.AddDays(60).ToString("o");
            var extend = await Client.PatchAsync(
                $"/api/campaign/{campaignId}/extend-deadline?newDeadline={Uri.EscapeDataString(newDeadline)}",
                null);
            extend.StatusCode.Should().Be(HttpStatusCode.OK);

            var status = await Client.PatchAsync(
                $"/api/campaign/{campaignId}/status?status={CampaignStatus.Active}",
                null);
            status.StatusCode.Should().Be(HttpStatusCode.OK);

            var money = await Client.PatchAsJsonAsync($"/api/campaign/{campaignId}/money",
                new UpdateCampaignMoneyDto { CampaignId = campaignId, Amount = 500 });
            money.StatusCode.Should().Be(HttpStatusCode.OK);

            ClearBearerToken();
            var increment = await Client.PatchAsJsonAsync($"/api/campaign/{campaignId}/increment-money",
                new IncrementCampaignMoneyDto { CampaignId = campaignId, Amount = 100 });
            increment.StatusCode.Should().Be(HttpStatusCode.OK);

            await SignUpAsSuperAdmin();
            (await Client.DeleteAsync($"/api/campaign/{campaignId}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.PatchAsync($"/api/campaign/{campaignId}/restore", null)).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Flow1_SharedCampaign_Create_AddOrg_Count_RemoveOrg_Update()
        {
            await SignUpAsSuperAdmin();
            var creatorOrg = await CreateOrganization("Creator Org");
            var partnerOrg = await CreateOrganization("Partner Org");
            var extraOrg = await CreateOrganization("Extra Org");

            var sharedId = await CreateSharedCampaign(
                creatorOrg.Id,
                new List<int> { creatorOrg.Id, partnerOrg.Id },
                "Shared Relief");

            ClearBearerToken();
            (await Client.GetAsync($"/api/campaign/shared/{sharedId}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/shared")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/campaign/shared/by-organization/{creatorOrg.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);

            await SignUpAsSuperAdmin();
            (await Client.PostAsync(
                $"/api/campaign/shared/{sharedId}/add-organization/{extraOrg.Id}", null))
                .StatusCode.Should().Be(HttpStatusCode.OK);

            ClearBearerToken();
            var count = await Client.GetAsync($"/api/campaign/shared/{sharedId}/organization-count");
            count.StatusCode.Should().Be(HttpStatusCode.OK);

            await SignUpAsSuperAdmin();
            (await Client.DeleteAsync(
                $"/api/campaign/shared/{sharedId}/remove-organization/{extraOrg.Id}"))
                .StatusCode.Should().Be(HttpStatusCode.OK);

            var updateShared = await Client.PutAsJsonAsync($"/api/campaign/shared/{sharedId}",
                new UpdateSharedCampaignDto
                {
                    Id = sharedId,
                    Title = "Shared Relief Updated",
                    Target = 25000
                });
            updateShared.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Flow2_PublicFilters_Statistics_Options()
        {
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization();
            await CreateSoloCampaign(org.Id, "Filter Campaign");
            ClearBearerToken();

            (await Client.GetAsync("/api/campaign/active")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/expiring-soon?daysThreshold=60")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/expired")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/search?term=Filter")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/campaign/filter/by-status?status={CampaignStatus.Preparing}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/campaign/filter/by-type?type={CampaignType.type1}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/filter/by-target-range?minTarget=1&maxTarget=100000")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/filter/by-achievement?minPercentage=0")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/ending-soon?remainingValue=60")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/options/statuses")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/options/types")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/solo")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/campaign/solo/by-status?status={CampaignStatus.Preparing}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/campaign/shared/by-status?status={CampaignStatus.Preparing}")).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Flow3_SuperAdmin_AutoExpire_Deleted_Bulk()
        {
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization();
            await CreateSoloCampaign(org.Id);

            (await Client.PostAsync("/api/campaign/auto-expire", null)).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/campaign/deleted")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.PatchAsync(
                $"/api/campaign/bulk/update-status?oldStatus={CampaignStatus.Preparing}&newStatus={CampaignStatus.Active}",
                null)).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.DeleteAsync("/api/campaign/bulk/delete-expired?daysAfterCompletion=0"))
                .StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
