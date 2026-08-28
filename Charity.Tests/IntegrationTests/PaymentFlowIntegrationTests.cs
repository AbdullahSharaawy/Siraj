using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TaskManagement.Tests.IntegrationTests;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.PaymentDTOs;

namespace Charity.Tests.IntegrationTests
{
    public class PaymentFlowIntegrationTests : IntegrationFlowTestBase
    {
        public PaymentFlowIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Flow0_CreatePayment_ReturnsIframeUrl()
        {
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization();
            var campaignId = await CreateSoloCampaign(org.Id);

            var donor = await SignUp();
            var token = await SignIn(donor);
            SetBearerToken(token);

            var response = await Client.PostAsJsonAsync("/api/payment/create", new CreatePaymentRequestDto
            {
                Amount = 100,
                CampaignId = campaignId,
                OrganizationId = org.Id
            });
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<string>>(JsonOptions);
            result!.Success.Should().BeTrue();
            result.Data.Should().NotBeNullOrEmpty();
            result.Data.Should().Contain("paymob");
        }

        [Fact]
        public async Task Flow1_Callback_ValidHmac_CreatesDonation()
        {
            await SignUpAsSuperAdmin();
            var org = await CreateOrganization();
            var campaignId = await CreateSoloCampaign(org.Id);
            var donor = await SignUp();
            ClearBearerToken();

            var createdAt = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);
            var transaction = new PaymobTransaction
            {
                Id = 999001,
                AmountCents = 15000,
                CreatedAt = createdAt,
                Currency = "EGP",
                ErrorOccured = false,
                HasParentTransaction = false,
                IntegrationId = 12345,
                Is3dSecure = false,
                IsAuth = false,
                IsCapture = false,
                IsRefunded = false,
                IsStandalonePayment = true,
                IsVoided = false,
                Owner = 1,
                Pending = false,
                Success = true,
                Order = new PaymobOrder { Id = 555 },
                SourceData = new SourceData { Pan = "2346", SubType = "Visa", Type = "card" },
                PaymentKeyClaims = new PaymentKeyClaims
                {
                    Extra = new Dictionary<string, object>
                    {
                        ["user_id"] = donor.Id,
                        ["campaign_id"] = campaignId.ToString()
                    }
                }
            };

            var hmac = ComputeHmac(transaction, CustomWebApplicationFactory.TestPaymobHmacKey);
            var wrapper = new PaymobCallbackWrapper { Type = "TRANSACTION", Obj = transaction };

            var response = await Client.PostAsJsonAsync($"/api/payment/callback?hmac={hmac}", wrapper);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            doc.RootElement.GetProperty("status").GetString().Should().Be("success");
            doc.RootElement.TryGetProperty("donation_id", out _).Should().BeTrue();
        }

        [Fact]
        public async Task Flow2_Callback_InvalidHmac_ReturnsUnauthorized()
        {
            ClearBearerToken();
            var wrapper = new PaymobCallbackWrapper
            {
                Type = "TRANSACTION",
                Obj = new PaymobTransaction
                {
                    Id = 1,
                    AmountCents = 100,
                    CreatedAt = DateTime.UtcNow,
                    Currency = "EGP",
                    Success = true,
                    Order = new PaymobOrder { Id = 1 },
                    SourceData = new SourceData()
                }
            };

            var response = await Client.PostAsJsonAsync("/api/payment/callback?hmac=invalid", wrapper);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private static string ComputeHmac(PaymobTransaction transaction, string secret)
        {
            var data = string.Concat(
                transaction.AmountCents,
                transaction.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.ffffff"),
                transaction.Currency,
                transaction.ErrorOccured.ToString().ToLowerInvariant(),
                transaction.HasParentTransaction.ToString().ToLowerInvariant(),
                transaction.Id,
                transaction.IntegrationId,
                transaction.Is3dSecure.ToString().ToLowerInvariant(),
                transaction.IsAuth.ToString().ToLowerInvariant(),
                transaction.IsCapture.ToString().ToLowerInvariant(),
                transaction.IsRefunded.ToString().ToLowerInvariant(),
                transaction.IsStandalonePayment.ToString().ToLowerInvariant(),
                transaction.IsVoided.ToString().ToLowerInvariant(),
                transaction.Order?.Id ?? 0,
                transaction.Owner,
                transaction.Pending.ToString().ToLowerInvariant(),
                transaction.SourceData?.Pan ?? "",
                transaction.SourceData?.SubType ?? "",
                transaction.SourceData?.Type ?? "",
                transaction.Success.ToString().ToLowerInvariant()
            );

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
