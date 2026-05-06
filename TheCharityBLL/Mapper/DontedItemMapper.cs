using Riok.Mapperly.Abstractions;
using TheCharityBLL.DTOs.DonatedItemDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper
{
    [Mapper]
    public partial class DontedItemMapper
    {
        public partial DonatedItem MapToDonatedItem(CreateDonatedItemDto createDonatedItemDto);

        //[MapProperty(nameof(DonatedItem.Organization.Name), nameof(DonatedItemResponseDto.OrganizationName))]
        //[MapProperty(nameof(DonatedItem.Donor.UserName), nameof(DonatedItemResponseDto.DonorName))]
        //[MapWith(nameof(GetMainImagePath), nameof(DonatedItemResponseDto.MainImagePath))]
        public DonatedItemResponseDto MapToDonatedItemResponseDto(DonatedItem donated)
        {
            return new DonatedItemResponseDto
            {
                Id = donated.Id,
                Name = donated.Name,
                Description = donated.Description,
                ItemCategory = donated.ItemCategory ?? default,
                IsAvailable = donated.IsAvailable ?? default,
                OrganizationId = donated.OrganizationId,
                OrganizationName = donated.Organization?.Name ?? string.Empty,
                DonorId = donated.DonorId,
                DonorName = donated.Donor?.UserName ?? string.Empty,
                MainImagePath = donated.Images?.FirstOrDefault()?.Path ?? string.Empty,
                RegistrationDate = donated.RegistrationDate,
                UpdatedOn = donated.UpdatedOn,
                IsDeleted = donated.IsDeleted
            };
        }

        public partial DonatedItemDetailsResponseDto MapToDonatedItemDetailsResponseDto(DonatedItem donated);
        public partial IEnumerable<DonatedItemDetailsResponseDto> MapToDonatedItemResponseDetailsDtos(IEnumerable<DonatedItem> donatedItems);
        public partial IEnumerable<DonatedItemResponseDto> MapToDonatedItemResponseDtos(IEnumerable<DonatedItem> donatedItems);
    }
}
