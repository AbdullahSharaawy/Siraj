using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheCharityBLL.DTOs.PaymentDTOs
{
    public class PaymobOrder
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("delivery_needed")]
        public bool DeliveryNeeded { get; set; }

        [JsonPropertyName("merchant")]
        public PaymobMerchant? Merchant { get; set; }

        [JsonPropertyName("collector")]
        public object? Collector { get; set; }

        [JsonPropertyName("amount_cents")]
        public long AmountCents { get; set; }

        [JsonPropertyName("shipping_data")]
        public ShippingData? ShippingData { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("is_payment_locked")]
        public bool IsPaymentLocked { get; set; }

        [JsonPropertyName("is_return")]
        public bool IsReturn { get; set; }

        [JsonPropertyName("is_cancel")]
        public bool IsCancel { get; set; }

        [JsonPropertyName("is_returned")]
        public bool IsReturned { get; set; }

        [JsonPropertyName("is_canceled")]
        public bool IsCancelled { get; set; }

        [JsonPropertyName("merchant_order_id")]
        public object? MerchantOrderId { get; set; }

        [JsonPropertyName("wallet_notification")]
        public object? WalletNotification { get; set; }

        [JsonPropertyName("paid_amount_cents")]
        public long PaidAmountCents { get; set; }

        [JsonPropertyName("notify_user_with_email")]
        public bool NotifyUserWithEmail { get; set; }

        [JsonPropertyName("items")]
        public List<object>? Items { get; set; }

        [JsonPropertyName("order_url")]
        public string? OrderUrl { get; set; }

        [JsonPropertyName("commission_fees")]
        public long CommissionFees { get; set; }

        [JsonPropertyName("delivery_fees_cents")]
        public long DeliveryFeesCents { get; set; }

        [JsonPropertyName("delivery_vat_cents")]
        public long DeliveryVatCents { get; set; }

        [JsonPropertyName("payment_method")]
        public string? PaymentMethod { get; set; }

        [JsonPropertyName("merchant_staff_tag")]
        public object? MerchantStaffTag { get; set; }

        [JsonPropertyName("api_source")]
        public string? ApiSource { get; set; }

        [JsonPropertyName("data")]
        public object? Data { get; set; }

        [JsonPropertyName("payment_status")]
        public string? PaymentStatus { get; set; }

        [JsonPropertyName("terminal_version")]
        public object? TerminalVersion { get; set; }

       
    }
}