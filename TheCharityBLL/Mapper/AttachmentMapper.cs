using Riok.Mapperly.Abstractions;
using TheCharityBLL.DTOs.AttachmentDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper
{
    [Mapper]
    public partial class AttachmentMapper
    {
        public partial Attachment MapToAttachment(CreateAttachmentDto createAttachmentDto); 
        public partial AttachmentResponseDto MapToAttachmentResponseDto(Attachment attachmentDto);
        public partial IEnumerable<AttachmentResponseDto> MapToAttachmentResponseDtos(IEnumerable<Attachment> attachments);
    }
}
