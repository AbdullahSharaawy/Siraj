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
        public  Task<OrganizationRole> AddOrganizationRoleAsync(int organizationId, string userId, OrganizationRoleType role);
        Task<bool> DeleteAsync(string userId, int organizationId);
        public  Task<IEnumerable<User>> GetOrganizationAdminsAsync(int organizationId);
        public Task<IEnumerable<OrganizationRole>> GetOrganizationRolesAsync(int organizationId);
        public  Task<bool> IsUserSubAdminAsync(int organizationId, string userId);
        public Task<bool> IsUserOrganizationAdminAsync(string userId);
    }
}
