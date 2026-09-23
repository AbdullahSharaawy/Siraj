using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityDAL.Database;
using TheCharityDAL.Entities;
using TheCharityDAL.Enums;
using TheCharityDAL.FilterModels;
using TheCharityDAL.Repositories.Abstraction;

namespace TheCharityDAL.Repositories.Implementation
{
    public class OrganizationRoleRepository: IOrganizationRoleRepository
    {
        private readonly TheCharityDbContext _dbContext;
        public OrganizationRoleRepository(TheCharityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OrganizationRole> AddOrganizationRoleAsync(int organizationId, string userId, OrganizationRoleType role)
        {
            // Check if user already has a role in this organization
            var existingRole = await _dbContext.OrganizationRoles
                .Where(r => r.OrganizationId == organizationId &&
                           r.UserId == userId &&
                           !r.IsDeleted)
                .FirstOrDefaultAsync();

            if (existingRole != null)
            {
                // Update existing role instead of creating new one
                existingRole = new OrganizationRole(organizationId, userId, role);
                _dbContext.OrganizationRoles.Update(existingRole);
            }
            else
            {
                var organizationRole = new OrganizationRole(organizationId, userId, role);
                _dbContext.OrganizationRoles.Add(organizationRole);
            }

            await _dbContext.SaveChangesAsync();

            return await _dbContext.OrganizationRoles
                .Where(r => r.OrganizationId == organizationId && r.UserId == userId)
                .FirstOrDefaultAsync()!;
        }



        public async Task<bool> DeleteAsync(string userId, int organizationId)
        {

            OrganizationRole? OrganizationRole = await GetByIdAsync(userId,  organizationId);
            if (OrganizationRole != null)
            {
                _dbContext.Remove(OrganizationRole);
                int RowAffected = await _dbContext.SaveChangesAsync();
                if (RowAffected > 0)
                    return true;
                return false;

            }
            else
                return false;


        }

      

        public async Task<(IEnumerable<OrganizationRole>, int)> GetAllAsync(QueryParameters query)

        {
            IQueryable<OrganizationRole> OrganizationRoles = _dbContext.OrganizationRoles
          .AsNoTracking()
          .OrderByDescending(p => p.RegistrationDate);

            int totalCount = await OrganizationRoles.CountAsync();

            var items = await OrganizationRoles
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            return (items, totalCount);

        }

        public async Task<OrganizationRole?> GetByIdAsync(string userId ,int organizationId)
        {

            return await _dbContext.OrganizationRoles.Where(p => p.UserId == userId && p.OrganizationId == organizationId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetOrganizationAdminsAsync(int organizationId)
        {
            return await _dbContext.OrganizationRoles
                .Where(p => p.OrganizationId == organizationId && p.Role == Enums.OrganizationRoleType.Admin)
                .Select(p => p.User)
                .ToListAsync();
        }
       
        public async Task RemoveSubAdminAsync(int organizationId, string userId)
        {
            var role = await _dbContext.OrganizationRoles
                .Where(r => r.OrganizationId == organizationId &&
                           r.UserId == userId &&
                           r.Role == OrganizationRoleType.SubAdmin &&
                           !r.IsDeleted)
                .FirstOrDefaultAsync();

            if (role != null)
            {
                role.Delete();
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsUserSubAdminAsync(int organizationId, string userId)
        {
            return await _dbContext.OrganizationRoles
                .AnyAsync(r => r.OrganizationId == organizationId &&
                              r.UserId == userId &&
                              r.Role == OrganizationRoleType.SubAdmin &&
                              !r.IsDeleted);
        }

        public async Task<bool> IsUserOrganizationAdminAsync(string userId)
        {
            return await _dbContext.OrganizationRoles
                .AnyAsync(r =>
                              r.UserId == userId && OrganizationRoleType.Admin == r.Role &&
                              !r.IsDeleted);
        }
        public async Task<IEnumerable<OrganizationRole>> GetOrganizationRolesAsync(int organizationId)
        {
            return await _dbContext.OrganizationRoles
                .Where(r => r.OrganizationId == organizationId && !r.IsDeleted)
                .Include(r => r.User)
                .ToListAsync();
        }
    }
}
