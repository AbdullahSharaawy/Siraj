using TheCharityBLL.DTOs.CampaignDTOs;
using TheCharityBLL.DTOs.OrganizationContactMethodDTOs;

namespace TheCharityBLL.DTOs.OrganizationDTOs
{
    public class OrganizationDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public ICollection<OrgContactMethodResponseDto> ContactMethods { get; set; }=new List<OrgContactMethodResponseDto>();
       
        public ICollection<CampaignResponseDto> Campaigns { get; set; }=new List<CampaignResponseDto>();

        public bool IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? PaymentId { get; set; }
        public string? AdminUserId { get; set; }
        public string AdminUserName { get; set; }
        public string AdminUserFullName { get; set; }
        public string AdminUserEmail { get; set; }
        public int SoloCampaignsCount { get; set; }
        public int SharedCampaignsCount { get; set; }
        public int TotalCampaignsCount { get; set; }
    }
}
