using Riok.Mapperly.Abstractions;
using TheCharityBLL.DTOs.ItemImageDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper
{
    [Mapper]
    public partial class ItemImageMapper
    {
        public partial ItemImage MapToItemImage(CreateItemImageDto createItemImageDto);
        public partial ItemImageResponseDto MapToItemImageResponseDto(ItemImage itemImage);
        public partial IEnumerable<ItemImageResponseDto> MapToItemImageResponseDtos(IEnumerable<ItemImage> itemImages);
    }
}
