using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskManagement.Tests.IntegrationTests;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.PaymentInfoDTOs;

namespace Charity.Tests.IntegrationTests
{
    public class PaymentInfoFlowIntegrationTests : IntegrationFlowTestBase
    {
        public PaymentInfoFlowIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Flow0_Create_Get_Has_Validate_Update_Delete_Restore()
        {
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization();

            var createResponse = await Client.PostAsJsonAsync("/api/paymentinfo", new CreatePaymentInfoDto
            {
                ApiKey = "test-api-key",
                IntegrationId = "12345",
                IframeId = "67890",
                HmacKey = "hmac-secret",
                OrganizationId = org.Id
            });
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createResult = await createResponse.Content
                .ReadFromJsonAsync<ServiceResponse<PaymentInfoResponseDto>>(JsonOptions);
            createResult!.Success.Should().BeTrue();
            createResult.Data.Should().NotBeNull();
            var paymentInfoId = createResult.Data!.Id;

            (await Client.GetAsync($"/api/paymentinfo/by-organization/{org.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/paymentinfo/{paymentInfoId}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/paymentinfo/has/{org.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Client.GetAsync($"/api/paymentinfo/validate/{org.Id}")).StatusCode.Should().Be(HttpStatusCode.OK);

            var update = await Client.PutAsJsonAsync($"/api/paymentinfo/{paymentInfoId}",
                new UpdatePaymentInfoDto
                {
                    ApiKey = "updated-api-key",
                    IntegrationId = "54321",
                    IframeId = "09876",
                    HmacKey = "updated-hmac",
                    OrganizationId = org.Id
                });
            update.StatusCode.Should().Be(HttpStatusCode.OK);

            var delete = await Client.DeleteAsync($"/api/paymentinfo/{paymentInfoId}");
            delete.StatusCode.Should().Be(HttpStatusCode.OK);

            var restore = await Client.GetAsync($"/api/paymentinfo/restore/{paymentInfoId}");
            restore.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
