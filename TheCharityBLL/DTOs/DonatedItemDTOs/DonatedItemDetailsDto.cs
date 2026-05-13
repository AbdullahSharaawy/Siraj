
using TheCharityBLL.DTOs.AttachmentDTOs;
using TheCharityBLL.DTOs.DonorDtos;
using TheCharityBLL.DTOs.ItemImageDTOs;
using TheCharityBLL.DTOs.OrganizationDTOs;
using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.DonatedItemDTOs
{
    public class DonatedItemDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public bool IsAvailable { get; set; }
        public int OrganizationId { get; set; }
        public OrganizationResponseDto? Organization { get; set; }
        public string DonorId { get; set; } = null!;
        public DonorResponceDto? Donor { get; set; }
        public List<ItemImageResponseDto>? Images { get; set; }
        public List<AttachmentResponseDto>? ItemAttachments { get; set; }
        public List<AttachmentResponseDto>? RecipientAttachments { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
