using Riok.Mapperly.Abstractions;
using TheCharityBLL.DTOs.OrganizationDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper
{
    [Mapper]
    public partial class OrganizationMaper
    {
        public partial Organization MapToOrganization(CreateOrganizationDto createOrganizationDto);
        public partial OrganizationDetailsResponseDto MapToOrganizationDetailsResponseDto(Organization organization);
        public partial OrganizationResponseDto MapToOrganizationResponseDto(Organization organization);
        public partial IEnumerable<OrganizationResponseDropDownListDto> MapToOrganizationResponseDropDownListDtos(IEnumerable<Organization> organizations);
        public partial IEnumerable<OrganizationResponseDto> MapToOrganizationResponseDtos(IEnumerable<Organization> organizations);
    }
}
