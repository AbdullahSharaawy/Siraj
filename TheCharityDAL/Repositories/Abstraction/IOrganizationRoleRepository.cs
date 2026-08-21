using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityDAL.Entities;
using TheCharityDAL.Enums;
using TheCharityDAL.FilterModels;

namespace TheCharityDAL.Repositories.Abstraction
{
    public interface IOrganizationRoleRepository
    {
        Task<OrganizationRole?> GetByIdAsync(string userId , int OrganizationId);
        Task<(IEnumerable<OrganizationRole>, int)> GetAllAsync(QueryParameters query);
        Task<OrganizationRole?> AddOrganizationRoleAsync(OrganizationRole OrganizationRole);
        Task<OrganizationRole?> UpdateAsync(OrganizationRole OrganizationRole);
        Task<bool> DeleteAsync(string userId, int organizationId);
    }
}
