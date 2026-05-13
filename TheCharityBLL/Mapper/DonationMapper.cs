using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs.CampaignDTOs;
using TheCharityBLL.DTOs.DonationDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper
{
    [Mapper]
    public partial class DonationMapper
    {
        // ===== CreateDto → Entity =====
        public partial Donation MapToDonation(CreateDonationDto createDonationDto);

        // ===== Entity → ResponseDto =====
        // Manual: Campaign is a navigation property that needs safe null handling
        public DonationResponseDto MapToDonationResponseDto(Donation donation)
        {
            return new DonationResponseDto
            {
                Id = donation.Id,
                Amount = donation.Amount ?? 0,
                UserId = donation.UserId,
                CampaignId = donation.CampaignId,
                Campaign = donation.Campaign is not null
                                        ? MapToCampaignResponseDto(donation.Campaign)
                                        : null,
                RegistrationDate = donation.RegistrationDate,
                UpdatedOn = donation.UpdatedOn,
                IsDeleted = donation.IsDeleted
            };
        }

        // ===== Collections =====
        public partial IEnumerable<DonationResponseDto> MapToDonationResponseDtos(
            IEnumerable<Donation> donations);

        // ===== Nested Campaign mapping =====
        // Mapperly auto-generates this — matches properties by name
        private partial CampaignResponseDto MapToCampaignResponseDto(Campaign campaign);
    }
}
