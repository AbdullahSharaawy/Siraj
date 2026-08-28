using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskManagement.Tests.IntegrationTests;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.OrganizationContactMethodDTOs;
using TheCharityBLL.DTOs.OrganizationDTOs;
using TheCharityDAL.Enums;

namespace Charity.Tests.IntegrationTests
{
    public class OrganizationFlowIntegrationTests : IntegrationFlowTestBase
    {
        public OrganizationFlowIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Flow0_Create_GetAll_GetById_GetDetails_Update()
        {
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization("Food Bank", "Giza");

            var getAllResponse = await Client.GetAsync("/api/organization");
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getAllResult = await getAllResponse.Content
                .ReadFromJsonAsync<ServiceResponse<List<OrganizationResponseDto>>>(JsonOptions);
            getAllResult!.Success.Should().BeTrue();
            getAllResult.Data.Should().Contain(o => o.Id == org.Id);

            var getByIdResponse = await Client.GetAsync($"/api/organization/{org.Id}");
            getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getByIdResult = await getByIdResponse.Content
                .ReadFromJsonAsync<ServiceResponse<OrganizationResponseDto>>(JsonOptions);
            getByIdResult!.Success.Should().BeTrue();
            getByIdResult.Data!.Name.Should().Be("Food Bank");

            var detailsResponse = await Client.GetAsync($"/api/organization/{org.Id}/details");
            detailsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updateResponse = await Client.PutAsJsonAsync($"/api/organization/{org.Id}",
                new UpdateOrganizationDto { Name = "Food Bank Updated", Address = "Cairo" });
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updateResult = await updateResponse.Content.ReadFromJsonAsync<ServiceResponse>(JsonOptions);
            updateResult!.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Flow1_Create_Delete_GetDeleted_Restore()
        {
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization();

            var deleteResponse = await Client.DeleteAsync($"/api/organization/{org.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deleteResult = await deleteResponse.Content.ReadFromJsonAsync<ServiceResponse>(JsonOptions);
            deleteResult!.Success.Should().BeTrue();

            var deletedListResponse = await Client.GetAsync("/api/organization/deleted");
            deletedListResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var restoreResponse = await Client.PatchAsync($"/api/organization/{org.Id}/restore", null);
            restoreResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var restoreResult = await restoreResponse.Content.ReadFromJsonAsync<ServiceResponse>(JsonOptions);
            restoreResult!.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Flow2_Search_Dropdown_Filters_Counts()
        {
            await SignUpAsSuperAdmin();
            var uniqueName = $"SearchOrg-{Guid.NewGuid():N}";
            await CreateOrganization(uniqueName, "Alexandria Street");
            ClearBearerToken();

            (await Client.GetAsync($"/api/organization/search?term={uniqueName}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/organization/dropdown")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/organization/filter/by-name?name={uniqueName}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/organization/filter/by-address?address=Alexandria")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/organization/recent?days=7")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/organization/count/total")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/organization/count/active")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/organization/campaigns/none")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync("/api/organization/payment/none")).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Flow3_ContactMethods_Create_Get_Update_Delete_Restore()
        {
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization();

            var createResponse = await Client.PostAsJsonAsync(
                $"/api/organization/contact-methods?organizationId={org.Id}",
                new CreateOrgContactMethodDto
                {
                    Value = "info@org.test",
                    Type = ContactType.Email,
                    CompanyId = org.Id
                });
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createResult = await createResponse.Content
                .ReadFromJsonAsync<ServiceResponse<OrgContactMethodResponseDto>>(JsonOptions);
            createResult!.Success.Should().BeTrue();
            var contactId = createResult.Data!.Id;

            (await Client.GetAsync($"/api/organization/{org.Id}/contact-methods")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/organization/contact-methods/{contactId}")).StatusCode.Should().Be(HttpStatusCode.OK);

            var updateResponse = await Client.PutAsJsonAsync(
                $"/api/organization/contact-methods/{contactId}?organizationId={org.Id}",
                new UpdateOrgContactMethodDto { Value = "support@org.test", Type = ContactType.Email });
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var deleteResponse = await Client.DeleteAsync(
                $"/api/organization/contact-methods/{contactId}?organizationId={org.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var restoreResponse = await Client.GetAsync(
                $"/api/organization/contact-methods/restore/{contactId}?organizationId={org.Id}");
            restoreResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            (await Client.GetAsync($"/api/organization/{org.Id}/contact-type?type=Email")).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Flow4_AssignAdmin_AddSubAdmin_Get_Check_Remove_Transfer()
        {
            var (superAdmin, _) = await SignUpAsSuperAdmin();
            var orgAdmin = await SignUp();
            var subAdmin = await SignUp();
            var transferTarget = await SignUp();
            var org = await CreateOrganization();

            var assignAdminResponse = await Client.PostAsJsonAsync(
                $"/api/organization/{org.Id}/admin",
                new AssignAdminRequest { UserId = orgAdmin.Id });
            assignAdminResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            (await Client.GetAsync($"/api/organization/{org.Id}/admin")).StatusCode.Should().Be(HttpStatusCode.OK);

            var addSubAdminResponse = await Client.PostAsJsonAsync(
                $"/api/organization/{org.Id}/sub-admins",
                new AssignAdminRequest { UserId = subAdmin.Id });
            addSubAdminResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            (await Client.GetAsync($"/api/organization/{org.Id}/sub-admins")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/organization/{org.Id}/sub-admins/{subAdmin.Id}/check")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.DeleteAsync($"/api/organization/{org.Id}/sub-admins/{subAdmin.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);

            var transferResponse = await Client.PostAsJsonAsync(
                $"/api/organization/{org.Id}/admin/transfer",
                new AssignAdminRequest { UserId = transferTarget.Id });
            transferResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            (await Client.DeleteAsync($"/api/organization/{org.Id}/admin")).StatusCode.Should().Be(HttpStatusCode.OK);
            superAdmin.Should().NotBeNull();
        }
    }
}
