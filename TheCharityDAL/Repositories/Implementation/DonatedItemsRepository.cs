using Microsoft.EntityFrameworkCore;
using TheCharityDAL.Database;
using TheCharityDAL.Entities;
using TheCharityDAL.Enums;
using TheCharityDAL.Repositories.Abstraction;

namespace TheCharityDAL.Repositories.Implementation
{
    public class DonatedItemsRepository : IDonatedItemsRepository
    {
        private readonly TheCharityDbContext _context;

        public DonatedItemsRepository(TheCharityDbContext context)
        {
            _context = context;
        }

        // ===== DonatedItem CRUD Operations =====
        public async Task<IEnumerable<DonatedItem>> GetAllDonatedItemsAsync(bool includeDeleted = false)
        {
            var query = _context.DonatedItems.AsQueryable();

            if (!includeDeleted)
                query = query.Where(di => di.IsDeleted == false);

            return await query
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .ToListAsync();
        }

        public async Task<DonatedItem?> GetDonatedItemByIdAsync(int id)
        {
            return await _context.DonatedItems
                .Where(di => di.Id == id && (di.IsDeleted == false))
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false))
                .Include(di => di.ItemAttachments.Where(a => a.IsDeleted == false))
                .Include(di => di.RecipientAttachments.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync();
        }

        public async Task<DonatedItem> AddDonatedItemAsync(DonatedItem donatedItem)
        {
            _context.DonatedItems.Add(donatedItem);
            await _context.SaveChangesAsync();
            return donatedItem;
        }

        public async Task<DonatedItem> UpdateDonatedItemAsync(DonatedItem donatedItem)
        {
            _context.Entry(donatedItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return donatedItem;
        }

        public async Task DeleteDonatedItemAsync(int id)
        {
            var donatedItem = await GetDonatedItemByIdAsync(id);
            if (donatedItem != null)
            {
                donatedItem.Delete();
                await _context.SaveChangesAsync();
            }
        }

        public async Task RestoreDonatedItemAsync(int id)
        {
            var donatedItem = await _context.DonatedItems.IgnoreQueryFilters()
                .Where(di => di.Id == id && di.IsDeleted == true)
                .FirstOrDefaultAsync();

            if (donatedItem != null)
            {
                donatedItem.Restore();
                await _context.SaveChangesAsync();
            }
        }

        // ===== DonatedItem Filtering & Search =====
        public async Task<IEnumerable<DonatedItem>> GetDonatedItemsByOrganizationAsync(int organizationId)
        {
            return await _context.DonatedItems
                .Where(di => di.OrganizationId == organizationId &&
                           (di.IsDeleted == false))
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> GetDonatedItemsByDonorAsync(string donorId)
        {
            return await _context.DonatedItems
                .Where(di => di.DonorId == donorId &&
                           (di.IsDeleted == false))
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> GetDonatedItemsByCategoryAsync(ItemCategory category)
        {
            return await _context.DonatedItems
                .Where(di => di.ItemCategory == category &&
                           (di.IsDeleted == false))
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> GetAvailableDonatedItemsAsync()
        {
            return await _context.DonatedItems
                .Where(di => di.IsAvailable == true &&
                           (di.IsDeleted == false))
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> GetUnavailableDonatedItemsAsync()
        {
            return await _context.DonatedItems
                .Where(di => di.IsAvailable == false &&
                           (di.IsDeleted == false))
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> SearchDonatedItemsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllDonatedItemsAsync();

            return await _context.DonatedItems
                .Where(di => (di.IsDeleted == false) &&
                           (di.Name != null && di.Name.Contains(searchTerm)) ||
                           (di.Description != null && di.Description.Contains(searchTerm)))
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> GetDeletedDonatedItemsAsync()
        {
            return await _context.DonatedItems
                .Where(di => di.IsDeleted == true)
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .ToListAsync();
        }

        // ===== Item Images Management =====
        public async Task<IEnumerable<ItemImage>> GetItemImagesAsync(int donatedItemId)
        {
            return await _context.ItemImages
                .Where(img => img.DonatedItemId == donatedItemId &&
                            (img.IsDeleted == false))
                .ToListAsync();
        }

        public async Task<ItemImage?> GetItemImageByIdAsync(int imageId)
        {
            return await _context.ItemImages
                .Where(img => img.Id == imageId &&
                            (img.IsDeleted == false))
                .FirstOrDefaultAsync();
        }

        public async Task<ItemImage> AddItemImageAsync(ItemImage itemImage)
        {
            _context.ItemImages.Add(itemImage);
            await _context.SaveChangesAsync();
            return itemImage;
        }

        public async Task<ItemImage> UpdateItemImageAsync(ItemImage itemImage)
        {
            _context.Entry(itemImage).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return itemImage;
        }

        public async Task DeleteItemImageAsync(int imageId)
        {
            var itemImage = await GetItemImageByIdAsync(imageId);
            if (itemImage != null)
            {
                itemImage.Delete();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetItemImageCountAsync(int donatedItemId)
        {
            return await _context.ItemImages
                .Where(img => img.DonatedItemId == donatedItemId &&
                            (img.IsDeleted == false))
                .CountAsync();
        }

        public async Task<ItemImage?> GetPrimaryItemImageAsync(int donatedItemId)
        {
            return await _context.ItemImages
                .Where(img => img.DonatedItemId == donatedItemId &&
                            (img.IsDeleted == false))
                .FirstOrDefaultAsync();
        }

        // ===== Attachments Management =====
        public async Task<IEnumerable<Attachment>> GetItemAttachmentsAsync(int donatedItemId)
        {
            return await _context.Attachments
                .Where(a => a.DonatedItemId == donatedItemId &&
                          (a.IsDeleted == false) && a.IsItemAttachment == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attachment>> GetRecipientAttachmentsAsync(int donatedItemId)
        {
            return await _context.Attachments
                .Where(a => a.DonatedItemId == donatedItemId &&
                          (a.IsDeleted == false) && a.IsItemAttachment == false)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attachment>> GetAllAttachmentsAsync(int donatedItemId)
        {
            return await _context.Attachments
                .Where(a => a.DonatedItemId == donatedItemId &&
                          (a.IsDeleted == false))
                .ToListAsync();
        }

        public async Task<Attachment?> GetAttachmentByIdAsync(int attachmentId)
        {
            return await _context.Attachments
                .Where(a => a.Id == attachmentId &&
                          (a.IsDeleted == false))
                .FirstOrDefaultAsync();
        }

        public async Task<Attachment> AddAttachmentAsync(Attachment attachment)
        {
            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<Attachment> UpdateAttachmentAsync(Attachment attachment)
        {
            _context.Entry(attachment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task DeleteAttachmentAsync(int attachmentId)
        {
            var attachment = await GetAttachmentByIdAsync(attachmentId);
            if (attachment != null)
            {
                attachment.Delete();
                await _context.SaveChangesAsync();
            }
        }

        // ===== DonatedItem Status Management =====
        public async Task<DonatedItem?> UpdateItemAvailabilityAsync(int itemId, bool isAvailable)
        {
            var donatedItem = await GetDonatedItemByIdAsync(itemId);
            if (donatedItem != null)
            {
                donatedItem.EditAvailability(isAvailable);
                await _context.SaveChangesAsync();
            }
            return donatedItem;
        }

        public async Task<DonatedItem?> MarkItemAsAvailableAsync(int itemId)
        {
            return await UpdateItemAvailabilityAsync(itemId, true);
        }

        public async Task<DonatedItem?> MarkItemAsUnavailableAsync(int itemId)
        {
            return await UpdateItemAvailabilityAsync(itemId, false);
        }

        public async Task<DonatedItem?> UpdateItemCategoryAsync(int itemId, ItemCategory category)
        {
            var donatedItem = await GetDonatedItemByIdAsync(itemId);
            if (donatedItem != null)
            {
                donatedItem.EditItemCategory(category);
                await _context.SaveChangesAsync();
            }
            return donatedItem;
        }

        // ===== Statistics & Analytics =====
        public async Task<int> GetTotalDonatedItemsCountAsync()
        {
            return await _context.DonatedItems
                .Where(di => di.IsDeleted == false)
                .CountAsync();
        }

        public async Task<int> GetAvailableDonatedItemsCountAsync()
        {
            return await _context.DonatedItems
                .Where(di => di.IsAvailable == true &&
                           (di.IsDeleted == false))
                .CountAsync();
        }

        public async Task<int> GetDonatedItemsCountByOrganizationAsync(int organizationId)
        {
            return await _context.DonatedItems
                .Where(di => di.OrganizationId == organizationId &&
                           (di.IsDeleted == false))
                .CountAsync();
        }

        public async Task<int> GetDonatedItemsCountByDonorAsync(string donorId)
        {
            return await _context.DonatedItems
                .Where(di => di.DonorId == donorId &&
                           (di.IsDeleted == false))
                .CountAsync();
        }

        public async Task<int> GetDonatedItemsCountByCategoryAsync(ItemCategory category)
        {
            return await _context.DonatedItems
                .Where(di => di.ItemCategory == category &&
                           (di.IsDeleted == false))
                .CountAsync();
        }

        public async Task<Dictionary<ItemCategory, int>> GetDonatedItemsCountToAllCategoriesAsync()
        {
            var result = await _context.DonatedItems
                .Where(di => di.IsDeleted == false)
                .GroupBy(di => di.ItemCategory)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            return result.ToDictionary(r => r.Category ?? ItemCategory.Other, r => r.Count);
        }

        public async Task<Dictionary<string, int>> GetTopDonorsAsync(int limit = 10)
        {
            var result = await _context.DonatedItems
                .Where(di => di.IsDeleted == false)
                .GroupBy(di => di.DonorId)
                .Select(g => new { DonorId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(limit)
                .ToListAsync();

            return result
                .Where(x => !string.IsNullOrEmpty(x.DonorId))
                .ToDictionary(x => x.DonorId, x => x.Count);
        }

        public async Task<Dictionary<int, int>> GetTopOrganizationsByDonationsAsync(int limit = 10)
        {
            var result = await _context.DonatedItems
                .Where(di => di.IsDeleted == false)
                .GroupBy(di => di.OrganizationId)
                .Select(g => new { OrganizationId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(limit)
                .ToListAsync();

            return result
                .ToDictionary(x => x.OrganizationId, x => x.Count);
        }

        // ===== Advanced Queries =====
        public async Task<IEnumerable<DonatedItem>> GetRecentDonatedItemsAsync(int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);

            return await _context.DonatedItems
                .Where(di => (di.IsDeleted == false) &&
                           di.RegistrationDate >= cutoffDate)
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .OrderByDescending(di => di.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> GetDonatedItemsWithoutImagesAsync()
        {
            var itemsWithImages = await _context.DonatedItems
                .Where(di => (di.IsDeleted == false) &&
                           di.Images != null && di.Images.Any())
                .Select(di => di.Id)
                .ToListAsync();

            return await _context.DonatedItems
                .Where(di => (di.IsDeleted == false) &&
                           !itemsWithImages.Contains(di.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> GetDonatedItemsWithAttachmentsAsync()
        {
            return await _context.DonatedItems
                .Where(di => (di.IsDeleted == false) &&
                           (di.ItemAttachments != null && di.ItemAttachments.Any()) ||
                           (di.RecipientAttachments != null && di.RecipientAttachments.Any()))
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> GetDonatedItemsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.DonatedItems
                .Where(di => (di.IsDeleted == false) &&
                           di.RegistrationDate >= startDate &&
                           di.RegistrationDate <= endDate)
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .OrderBy(di => di.RegistrationDate)
                .ToListAsync();
        }

        // ===== Bulk Operations =====
        public async Task<int> BulkUpdateItemCategoriesAsync(ItemCategory oldCategory, ItemCategory newCategory)
        {
            var items = await _context.DonatedItems
                .Where(di => di.ItemCategory == oldCategory &&
                           (di.IsDeleted == false))
                .ToListAsync();

            foreach (var item in items)
            {
                item.EditItemCategory(newCategory);
            }

            await _context.SaveChangesAsync();
            return items.Count;
        }

        public async Task<int> BulkMarkItemsAsUnavailableAsync(int organizationId)
        {
            var items = await _context.DonatedItems
                .Where(di => di.OrganizationId == organizationId &&
                           di.IsAvailable == true &&
                           (di.IsDeleted == false))
                .ToListAsync();

            foreach (var item in items)
            {
                item.EditAvailability(false);
            }

            await _context.SaveChangesAsync();
            return items.Count;
        }

        public async Task<int> DeleteOldDonatedItemsAsync(int daysOld = 365)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysOld);

            var items = await _context.DonatedItems
                .Where(di => (di.IsDeleted == false) &&
                           di.RegistrationDate < cutoffDate &&
                           di.IsAvailable == false)
                .ToListAsync();

            foreach (var item in items)
            {
                item.Delete();
            }

            await _context.SaveChangesAsync();
            return items.Count;
        }

        // ===== Validation & Checks =====
        public async Task<bool> DonatedItemExistsAsync(int id)
        {
            return await _context.DonatedItems.AnyAsync(di => di.Id == id);
        }

        public async Task<bool> IsDonatedItemAvailableAsync(int id)
        {
            var item = await GetDonatedItemByIdAsync(id);
            return item?.IsAvailable == true;
        }

        public async Task<bool> HasDonatedItemImagesAsync(int id)
        {
            return await _context.ItemImages
                .AnyAsync(img => img.DonatedItemId == id &&
                               (img.IsDeleted == false));
        }

        public async Task<bool> HasDonatedItemAttachmentsAsync(int id)
        {
            return await _context.Attachments
                .AnyAsync(a => a.DonatedItemId == id &&
                             (a.IsDeleted == false));
        }

        public async Task<bool> IsDonorAsync(string donorId)
        {
            return await _context.Users.AnyAsync(u => u.Id == donorId);
        }

        // ===== Eager Loading =====
        public async Task<DonatedItem?> GetDonatedItemWithImagesAsync(int id)
        {
            return await _context.DonatedItems
                .Where(di => di.Id == id && (di.IsDeleted == false))
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .FirstOrDefaultAsync();
        }

        public async Task<DonatedItem?> GetDonatedItemWithAttachmentsAsync(int id)
        {
            return await _context.DonatedItems
                .Where(di => di.Id == id && (di.IsDeleted == false))
                .Include(di => di.ItemAttachments.Where(a => a.IsDeleted == false))
                .Include(di => di.RecipientAttachments.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync();
        }

        public async Task<DonatedItem?> GetDonatedItemWithDonorAndOrganizationAsync(int id)
        {
            return await _context.DonatedItems
                .Where(di => di.Id == id && (di.IsDeleted == false))
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .FirstOrDefaultAsync();
        }

        // ===== Dashboard & Reporting =====
        public async Task<IEnumerable<DonatedItem>> GetMostRecentDonatedItemsAsync(int limit = 10)
        {
            return await _context.DonatedItems
                .Where(di => di.IsDeleted == false)
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .OrderByDescending(di => di.RegistrationDate)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Dictionary<DateTime, int>> GetDonatedItemsTrendAsync(int days = 30)
        {
            var startDate = DateTime.Now.AddDays(-days).Date;

            var trend = await _context.DonatedItems
                .Where(di => (di.IsDeleted == false) &&
                           di.RegistrationDate >= startDate)
                .GroupBy(di => di.RegistrationDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return trend.ToDictionary(x => x.Date, x => x.Count);
        }

        // ===== Donor Management =====
        public async Task<IEnumerable<DonatedItem>> GetDonorDonatedItemsHistoryAsync(string donorId)
        {
            return await _context.DonatedItems
                .Where(di => di.DonorId == donorId &&
                           (di.IsDeleted == false))
                .Include(di => di.Organization)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .OrderByDescending(di => di.RegistrationDate)
                .ToListAsync();
        }

        public async Task<int> GetDonorTotalDonatedItemsCountAsync(string donorId)
        {
            return await _context.DonatedItems
                .Where(di => di.DonorId == donorId &&
                           (di.IsDeleted == false))
                .CountAsync();
        }

        public async Task<ItemCategory> GetDonorMostCommonCategoryAsync(string donorId)
        {
            var categories = await _context.DonatedItems
                .Where(di => di.DonorId == donorId &&
                           (di.IsDeleted == false))
                .GroupBy(di => di.ItemCategory)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return categories.FirstOrDefault()?.Category ?? ItemCategory.Other;
        }

        // ===== Organization Management =====
        public async Task<IEnumerable<DonatedItem>> GetOrganizationInventoryAsync(int organizationId)
        {
            return await GetDonatedItemsByOrganizationAsync(organizationId);
        }

        public async Task<int> GetOrganizationAvailableItemsCountAsync(int organizationId)
        {
            return await _context.DonatedItems
                .Where(di => di.OrganizationId == organizationId &&
                           di.IsAvailable == true &&
                           (di.IsDeleted == false))
                .CountAsync();
        }

        public async Task<Dictionary<ItemCategory, int>> GetOrganizationInventoryByCategoryAsync(int organizationId)
        {
            var result = await _context.DonatedItems
                .Where(di => di.OrganizationId == organizationId &&
                           (di.IsDeleted == false))
                .GroupBy(di => di.ItemCategory)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            return result.ToDictionary(r => r.Category ?? ItemCategory.Other, r => r.Count);
        }

        // ===== File Management =====
        public async Task<long> GetTotalFileSizeAsync(int donatedItemId)
        {
            var attachments = await GetAllAttachmentsAsync(donatedItemId);
            return attachments.Sum(a => a.FileSize ?? 0);
        }

        public async Task<long> GetTotalStorageUsedAsync()
        {
            var totalSize = await _context.Attachments
                .Where(a => a.IsDeleted == false)
                .SumAsync(a => a.FileSize ?? 0);

            return totalSize;
        }

        public async Task<IEnumerable<Attachment>> GetLargeAttachmentsAsync(long sizeThreshold = 10485760)
        {
            return await _context.Attachments
                .Where(a => a.FileSize >= sizeThreshold &&
                          (a.IsDeleted == false))
                .ToListAsync();
        }

        // ===== Category Management =====
        public async Task<IEnumerable<DonatedItem>> GetItemsByMultipleCategoriesAsync(IEnumerable<ItemCategory> categories)
        {
            return await _context.DonatedItems
                .Where(di => categories.Contains(di.ItemCategory ?? ItemCategory.Other) &&
                           (di.IsDeleted == false))
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .ToListAsync();
        }

        public async Task<Dictionary<ItemCategory, decimal>> GetCategoryDistributionPercentageAsync()
        {
            var totalCount = await GetTotalDonatedItemsCountAsync();
            if (totalCount == 0)
                return new Dictionary<ItemCategory, decimal>();

            var categoryCounts = await GetDonatedItemsCountToAllCategoriesAsync();

            return categoryCounts.ToDictionary(
                kvp => kvp.Key,
                kvp => (decimal)kvp.Value / totalCount * 100
            );
        }

        public async Task<IEnumerable<ItemCategory>> GetMostPopularCategoriesAsync(int limit = 2)
        {
            var categoryCounts = await GetDonatedItemsCountToAllCategoriesAsync();
            return categoryCounts
                .OrderByDescending(kvp => kvp.Value)
                .Take(limit)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        // ===== Search & Filter Combinations =====
        public async Task<IEnumerable<DonatedItem>> SearchAvailableItemsByCategoryAsync(string searchTerm, ItemCategory? category = null)
        {
            var query = _context.DonatedItems
                .Where(di => di.IsAvailable == true &&
                           (di.IsDeleted == false));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(di => di.Name != null && di.Name.Contains(searchTerm) ||
                                         di.Description != null && di.Description.Contains(searchTerm));
            }

            if (category.HasValue)
            {
                query = query.Where(di => di.ItemCategory == category);
            }

            return await query
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> GetItemsByOrganizationAndCategoryAsync(int organizationId, ItemCategory category)
        {
            return await _context.DonatedItems
                .Where(di => di.OrganizationId == organizationId &&
                           di.ItemCategory == category &&
                           (di.IsDeleted == false))
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .ToListAsync();
        }

        public async Task<IEnumerable<DonatedItem>> GetItemsByDonorAndDateRangeAsync(string donorId, DateTime startDate, DateTime endDate)
        {
            return await _context.DonatedItems
                .Where(di => di.DonorId == donorId &&
                           (di.IsDeleted == false) &&
                           di.RegistrationDate >= startDate &&
                           di.RegistrationDate <= endDate)
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .Include(di => di.Images.Where(img => img.IsDeleted == false && img.IsMain==true))
                .OrderBy(di => di.RegistrationDate)
                .ToListAsync();
        }

        // ===== Transfer Operations =====
        public async Task<DonatedItem?> TransferItemToOrganizationAsync(int itemId, int newOrganizationId)
        {
            var donatedItem = await GetDonatedItemByIdAsync(itemId);
            if (donatedItem != null)
            {
                donatedItem.EditOrganization(newOrganizationId);
                await _context.SaveChangesAsync();
            }
            return donatedItem;
        }

        public async Task<DonatedItem?> UpdateItemDonorAsync(int itemId, string newDonorId)
        {
            var donatedItem = await GetDonatedItemByIdAsync(itemId);
            if (donatedItem != null)
            {
                donatedItem.EditDonor(newDonorId);
                await _context.SaveChangesAsync();
            }
            return donatedItem;
        }

        // ===== Audit & History =====
        public async Task<IEnumerable<DonatedItem>> GetRecentlyUpdatedItemsAsync(int hours = 24)
        {
            var cutoffDate = DateTime.Now.AddHours(-hours);

            return await _context.DonatedItems
                .Where(di => (di.IsDeleted == false) &&
                           di.UpdatedOn >= cutoffDate)
                .Include(di => di.Organization)
                .Include(di => di.Donor)
                .OrderByDescending(di => di.UpdatedOn)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetActivityByDonorAsync(int days = 30)
        {
            var startDate = DateTime.Now.AddDays(-days);

            var activity = await _context.DonatedItems
                .Where(di => (di.IsDeleted == false) &&
                           di.RegistrationDate >= startDate)
                .GroupBy(di => di.DonorId)
                .Select(g => new { DonorId = g.Key, ActivityCount = g.Count() })
                .OrderByDescending(x => x.ActivityCount)
                .ToListAsync();

            return activity
                .Where(x => !string.IsNullOrEmpty(x.DonorId))
                .ToDictionary(x => x.DonorId, x => x.ActivityCount);
        }

     
    }
}
