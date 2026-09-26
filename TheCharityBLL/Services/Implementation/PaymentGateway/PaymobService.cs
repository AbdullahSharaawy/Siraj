using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.PaymentDTOs;
using TheCharityBLL.Services.Abstraction.Payment;

namespace TheCharityBLL.Services.Implementation.PaymentGateway
{
    public class PaymobService : IPaymobService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymobService> _logger;
        private readonly IPaymentInfoService _paymentInfoService;
        private readonly IConfiguration _configuration;

        public PaymobService(
            ILogger<PaymobService> logger,
            IPaymentInfoService paymentInfoService,
            IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _paymentInfoService = paymentInfoService;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> CreatePayment(decimal amount, string currency = "EGP")
        {
            string result = await CreatePayment(amount, metadata: null, billingData: null, currency);
            return new ServiceResponse<string>
            {
                Success = true,
                Data = result,
                Message = "Payment created successfully"
            };
        }

        public async Task<string> CreatePayment(decimal amount, PaymentOrderMetadata? metadata, BillingData? billingData, string currency = "EGP")
        {
            // ── Step 1: Authentication ──────────────────────────────────────
            string? apiKey = null;
            string? integrationId = null;
            string? iframeId = null;

            if (metadata != null && metadata.OrganizationId > 0)
            {
                try
                {
                    var paymentKeys = await _paymentInfoService.GetPaymentInfoByOrganizationIdAsync(metadata.OrganizationId);
                    if (paymentKeys?.Data != null && !string.IsNullOrEmpty(paymentKeys.Data.ApiKey))
                    {
                        apiKey = paymentKeys.Data.ApiKey;
                        integrationId = paymentKeys.Data.IntegrationId;
                        iframeId = paymentKeys.Data.IframeId;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not fetch payment info for organization {OrganizationId}, falling back to platform settings.", metadata.OrganizationId);
                }
            }

            // Fallback to platform settings
            apiKey ??= _configuration["Paymob:ApiKey"];
            integrationId ??= _configuration["Paymob:IntegrationId"];
            iframeId ??= _configuration["Paymob:IframeId"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(integrationId) || string.IsNullOrEmpty(iframeId))
            {
                throw new InvalidOperationException("Paymob credentials (ApiKey, IntegrationId, IframeId) are not configured.");
            }

            var authBody = JsonSerializer.Serialize(new { api_key = apiKey });

            var authResponse = await _httpClient.PostAsync(
                "https://accept.paymob.com/api/auth/tokens",
                new StringContent(authBody, Encoding.UTF8, "application/json")
            );

            var authContent = await authResponse.Content.ReadAsStringAsync();
            _logger.LogInformation("Auth response: {Content}", authContent);

            if (!authResponse.IsSuccessStatusCode)
                throw new Exception($"Paymob auth failed ({authResponse.StatusCode}): {authContent}");

            var authJson = JsonDocument.Parse(authContent).RootElement;

            if (!authJson.TryGetProperty("token", out var authTokenEl))
                throw new Exception($"Paymob auth response missing 'token'. Response: {authContent}");

            var authToken = authTokenEl.GetString()!;

            var orderBody = JsonSerializer.Serialize(new
            {
                auth_token = authToken,
                amount_cents = (int)(amount * 100),
                currency,
                delivery_needed = false,
                items = Array.Empty<object>()
            });

            var orderRequest = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/ecommerce/orders")
            {
                Content = new StringContent(orderBody, Encoding.UTF8, "application/json")
            };
            orderRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            var orderResponse = await _httpClient.SendAsync(orderRequest);

            var orderContent = await orderResponse.Content.ReadAsStringAsync();
            _logger.LogInformation("Order response: {Content}", orderContent);

            if (!orderResponse.IsSuccessStatusCode)
                throw new Exception($"Paymob order creation failed ({orderResponse.StatusCode}): {orderContent}");

            var orderJson = JsonDocument.Parse(orderContent).RootElement;

            if (!orderJson.TryGetProperty("id", out var orderIdEl))
                throw new Exception($"Paymob order response missing 'id'. Response: {orderContent}");

            var orderId = orderIdEl.GetInt64();

            var resolvedBilling = ResolveBillingData(billingData);

            if (!int.TryParse(integrationId, out var integrationIdInt))
            {
                throw new InvalidOperationException($"Invalid Paymob IntegrationId: '{integrationId}'. Must be a valid integer.");
            }

            // ── Step 3: Payment Key ─────────────────────────────────────────
            var paymentBody = JsonSerializer.Serialize(new
            {
                auth_token = authToken,
                amount_cents = (int)(amount * 100),
                expiration = 3600,
                order_id = orderId,
                currency,
                integration_id = integrationIdInt,

                billing_data = resolvedBilling,
                extra = new Dictionary<string, string>
                {
                    ["user_id"] = metadata?.UserId ?? "",
                    ["campaign_id"] = metadata?.CampaignId.ToString() ?? "",
                    ["organization_id"] = metadata?.OrganizationId.ToString() ?? ""
                }
            });

            var paymentResponse = await _httpClient.PostAsync(
                "https://accept.paymob.com/api/acceptance/payment_keys",
                new StringContent(paymentBody, Encoding.UTF8, "application/json")
            );

            var paymentContent = await paymentResponse.Content.ReadAsStringAsync();
            _logger.LogInformation("Payment key response: {Content}", paymentContent);

            if (!paymentResponse.IsSuccessStatusCode)
                throw new Exception($"Paymob payment key failed ({paymentResponse.StatusCode}): {paymentContent}");

            var paymentJson = JsonDocument.Parse(paymentContent).RootElement;

            if (!paymentJson.TryGetProperty("token", out var paymentTokenEl))
                throw new Exception($"Paymob payment key response missing 'token'. Response: {paymentContent}");

            var paymentKey = paymentTokenEl.GetString()!;

            // ── Step 4: Return iFrame URL ───────────────────────────────────
            return $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";
        }

        private static object ResolveBillingData(BillingData? billing)
        {
            var phone = !string.IsNullOrWhiteSpace(billing?.PhoneNumber) && billing.PhoneNumber != "NA"
                ? billing.PhoneNumber
                : "+201000000000";

            if (billing is null)
            {
                return new
                {
                    first_name = "Donor",
                    last_name = "Contributor",
                    email = "donor@thecharity.org",
                    phone_number = phone,
                    apartment = "NA",
                    floor = "NA",
                    street = "Cairo",
                    building = "NA",
                    shipping_method = "NA",
                    postal_code = "11511",
                    city = "Cairo",
                    country = "EG",
                    state = "Cairo"
                };
            }

            return new
            {
                first_name = string.IsNullOrWhiteSpace(billing.FirstName) || billing.FirstName == "NA" ? "Donor" : billing.FirstName,
                last_name = string.IsNullOrWhiteSpace(billing.LastName) || billing.LastName == "NA" ? "Contributor" : billing.LastName,
                email = string.IsNullOrWhiteSpace(billing.Email) || billing.Email == "NA" ? "donor@thecharity.org" : billing.Email,
                phone_number = phone,
                apartment = billing.Apartment ?? "NA",
                floor = billing.Floor ?? "NA",
                street = string.IsNullOrWhiteSpace(billing.Street) || billing.Street == "NA" ? "Cairo" : billing.Street,
                building = billing.Building ?? "NA",
                shipping_method = "NA",
                postal_code = string.IsNullOrWhiteSpace(billing.PostalCode) || billing.PostalCode == "NA" ? "11511" : billing.PostalCode,
                city = string.IsNullOrWhiteSpace(billing.City) || billing.City == "NA" ? "Cairo" : billing.City,
                country = string.IsNullOrWhiteSpace(billing.Country) ? "EG" : billing.Country,
                state = string.IsNullOrWhiteSpace(billing.State) || billing.State == "NA" ? "Cairo" : billing.State
            };
        }

       
    }
}