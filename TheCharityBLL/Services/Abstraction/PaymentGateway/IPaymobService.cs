using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs.PaymentDTOs;

namespace TheCharityBLL.Services.Abstraction.Payment
{
    public interface IPaymobService
    {
        public Task<string> CreatePayment(decimal amount, string currency = "EGP");
        Task<string> CreatePayment(decimal amount, PaymentOrderMetadata? metadata, BillingData? billingData, string currency = "EGP");
    }
}
