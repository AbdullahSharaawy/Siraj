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
        public int? PaymentId { get; set; }
        public string? AdminUserId { get; set; }
        public string AdminUserName { get; set; }
        public string AdminUserFullName { get; set; }
        public string AdminUserEmail { get; set; }
    }
}
