
using TheCharityBLL.DTOs.AttachmentDTOs;
using TheCharityBLL.DTOs.ItemImageDTOs;
using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.DonatedItemDTOs
{
    public class DonatedItemResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public bool IsAvailable { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }=null!;
        public string DonorId { get; set; } = null!;
        public string DonorName { get; set; } = null!;
        //public string? MainImagePath { get; set; }
        public ICollection<ItemImageResponseDto>? Images { get; set; }=new List<ItemImageResponseDto>();
        public ICollection<AttachmentResponseDto>? ItemAttachments { get; set; }=new List<AttachmentResponseDto>();
        public ICollection<AttachmentResponseDto>? RecipientAttachments { get; set; } = new List<AttachmentResponseDto>();
        public DateTime RegistrationDate { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

    }
}
