using Riok.Mapperly.Abstractions;
using TheCharityBLL.DTOs.OrganizationContactMethodDTOs;
using TheCharityBLL.DTOs.OrganizationDTOs;
using TheCharityBLL.DTOs.PaymentInfoDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper
{
    [Mapper]
    public partial class OrganizationMapper
    {
        public partial Organization MapToOrganization(CreateOrganizationDto createOrganizationDto);
        public partial OrganizationResponseDto MapToOrganizationResponseDto(Organization organization);
        public partial OrganizationDetailsDto MapToOrganizationDetailsDto(Organization organization);
        public partial IEnumerable<OrganizationDropDownListDto> MapToOrganizationDropDownListDtos(IEnumerable<Organization> organizations);
        public partial IEnumerable<OrganizationResponseDto> MapToOrganizationResponseDtos(IEnumerable<Organization> organizations);
        public partial OrganizationContactMethod MapToOrganizationContactMethod(CreateOrgContactMethodDto contactMethod);
        public partial OrgContactMethodResponseDto MapToOrganizationContactMethodResponseDto(OrganizationContactMethod contactMethod);
        public partial IEnumerable<OrgContactMethodResponseDto> MapToOrganizationContactMethodResponseDtos(IEnumerable<OrganizationContactMethod> contactMethods);
        public partial PaymentInfo MapToPaymentInfo(CreatePaymentInfoDto paymentInfo);
        public partial PaymentInfoResponseDto MapToPaymentInfoResponseDto(PaymentInfo paymentInfo);
        public partial IEnumerable<PaymentInfoResponseDto> MapToPaymentInfoResponseDto(IEnumerable<PaymentInfo> paymentInfo);
    }
}
