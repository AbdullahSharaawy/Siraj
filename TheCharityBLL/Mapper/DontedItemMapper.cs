
using Riok.Mapperly.Abstractions;
using TheCharityBLL.DTOs.AttachmentDTOs;
using TheCharityBLL.DTOs.DonatedItemDTOs;
using TheCharityBLL.DTOs.ItemImageDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper
{
    [Mapper]
    public partial class DontedItemMapper
    {
        [MapperIgnoreSource(nameof(CreateDonatedItemDto.ImageFiles))]
        [MapperIgnoreSource(nameof(CreateDonatedItemDto.AttachmentFiles))]
        public partial DonatedItem MapToDonatedItem(CreateDonatedItemDto createDonatedItemDto);

        [MapProperty(nameof(DonatedItem.Organization.Name), nameof(DonatedItemResponseDto.OrganizationName))]
        [MapProperty(nameof(DonatedItem.Donor.UserName), nameof(DonatedItemResponseDto.DonorName))]
        public partial DonatedItemResponseDto MapToDonatedItemResponseDto(DonatedItem donated);
        public partial DonatedItemDetailsDto MapToDonatedItemDetailsDto(DonatedItem donated);
        public partial IEnumerable<DonatedItemDetailsDto> MapToDonatedItemDetailsDtos(IEnumerable<DonatedItem> donatedItems);
        public partial IEnumerable<DonatedItemResponseDto> MapToDonatedItemResponseDtos(IEnumerable<DonatedItem> donatedItems);

        public partial Attachment MapToAttachment(CreateAttachmentDto createAttachmentDto);
        public partial AttachmentResponseDto MapToAttachmentResponseDto(Attachment attachmentDto);
        public partial IEnumerable<AttachmentResponseDto> MapToAttachmentResponseDtos(IEnumerable<Attachment> attachments);

        public partial ItemImage MapToItemImage(CreateItemImageDto createItemImageDto);
        public partial ItemImageResponseDto MapToItemImageResponseDto(ItemImage itemImage);
        public partial IEnumerable<ItemImageResponseDto> MapToItemImageResponseDtos(IEnumerable<ItemImage> itemImages);

    }
}
