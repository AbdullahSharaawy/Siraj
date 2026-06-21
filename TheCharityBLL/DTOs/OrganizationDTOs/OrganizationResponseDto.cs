using TheCharityBLL.DTOs.OrganizationContactMethodDTOs;
using TheCharityBLL.DTOs.PaymentInfoDTOs;

namespace TheCharityBLL.DTOs.OrganizationDTOs
{
    public class OrganizationResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public PaymentInfoResponseDto? PaymentInfo { get; set; }
        public ICollection<OrgContactMethodResponseDto> ContactMethods { get; set; } = new List<OrgContactMethodResponseDto>();
        public bool IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
