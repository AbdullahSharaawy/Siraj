using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs.PaymentInfoDTOs;

namespace TheCharityBLL.Services.Abstraction.Payment
{
    public interface IPaymentInfoService
    {
        Task<PaymentInfoResponseDto?> GetPaymentInfoByOrganizationIdAsync(int organizationId);
        Task<PaymentInfoResponseDto?> GetPaymentInfoByIdAsync(int paymentInfoId);
        Task<PaymentInfoResponseDto?> CreatePaymentInfoAsync(CreatePaymentInfoDto dto);
        Task<PaymentInfoResponseDto> UpdatePaymentInfoAsync(int paymentInfoId, UpdatePaymentInfoDto dto);
        Task DeletePaymentInfoAsync(int paymentInfoId);
        Task<bool> RestorePaymentInfoAsync(int paymentInfoId);

        Task<bool> HasPaymentInfoAsync(int organizationId);
        Task<bool> ValidatePaymentInfoAsync(int organizationId);
    }
}
