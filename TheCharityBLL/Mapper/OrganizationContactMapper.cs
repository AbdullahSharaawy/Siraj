using Riok.Mapperly.Abstractions;
using TheCharityBLL.DTOs.OrganizationContactMethodDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper
{
    [Mapper]
    public partial class OrganizationContactMapper
    {
        public partial OrganizationContactMethodResponseDto MapToOrganizationContactMethodResponseDto(OrganizationContactMethod contactMethod);
        public partial IEnumerable<OrganizationContactMethodResponseDto> MapToOrganizationContactMethodResponseDtos(IEnumerable<OrganizationContactMethod> contactMethods);
        public partial OrganizationContactMethod MapToOrganizationContactMethod(CreateOrganizationContactMethodDto contactMethod);
    }
}
