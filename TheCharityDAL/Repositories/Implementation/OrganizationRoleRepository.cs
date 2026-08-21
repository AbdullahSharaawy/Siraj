using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityDAL.Database;
using TheCharityDAL.Entities;
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
        public async Task<OrganizationRole?> AddOrganizationRoleAsync(OrganizationRole OrganizationRole)
        {
            var existingOrganizationRole = await _dbContext.OrganizationRoles.FirstOrDefaultAsync(p => p.UserId == OrganizationRole.UserId && p.OrganizationId==OrganizationRole.OrganizationId);
            if (existingOrganizationRole != null)
            {
                return null;
            }
            _dbContext.Add(OrganizationRole);

            int RowAffected = await _dbContext.SaveChangesAsync();
            if (RowAffected > 0)
                return OrganizationRole;
            return null;
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

       
        public async Task<OrganizationRole?> UpdateAsync(OrganizationRole OrganizationRole)
        {
            _dbContext.Update(OrganizationRole);
            int RowAffected = await _dbContext.SaveChangesAsync();
            if (RowAffected > 0)
                return OrganizationRole;
            return null;
        }

    }
}
