using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.AttachmentDTOs;
using TheCharityBLL.DTOs.DonatedItemDTOs;
using TheCharityBLL.DTOs.ItemImageDTOs;
using TheCharityDAL.Entities;
using TheCharityDAL.Enums;

namespace TheCharityBLL.Services.Abstraction
{
    public interface IDonatedItemService
    {
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetAllDonatedItems(bool includeDeleted = false);
        Task<ServiceResponse<DonatedItemResponseDto>> GetDonatedItemById(int id);
        //Task<ServiceResponse<DonatedItemResponseDto>> GetDonatedItemDetails(int id);
        Task<ServiceResponse<DonatedItemResponseDto>> CreateDonatedItem(CreateDonatedItemDto donatedItem);
        Task<ServiceResponse<DonatedItemResponseDto>> UpdateDonatedItem(int id, UpdateDonatedItemDto donatedItem);
        Task<ServiceResponse<bool>> DeleteDonatedItem(int id);
        Task<ServiceResponse<bool>> RestoreDonatedItem(int id);

        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsByOrganization(int organizationId);
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsByDonor(string donorId);
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsByCategory(ItemCategory category);
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetAvailableDonatedItems();
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetUnavailableDonatedItems();
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> SearchDonatedItems(string searchTerm);
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDeletedDonatedItems();

        Task<ServiceResponse<IEnumerable<ItemImageResponseDto>>> GetItemImages(int donatedItemId);
        Task<ServiceResponse<ItemImageResponseDto>> GetItemImageById(int imageId);
        Task<ServiceResponse<ItemImageResponseDto>> CreateItemImage(CreateItemImageDto itemImage);
        //Task<ServiceResponse<ItemImageResponseDto>> UpdateItemImage(UpdateItemImageDto itemImage);
        Task<ServiceResponse<bool>> DeleteItemImage(int imageId);
        Task<ServiceResponse<int>> GetItemImageCountByDonatedItem(int donatedItemId);
        Task<ServiceResponse<ItemImageResponseDto>> GetPrimaryItemImageForDonatedItem(int donatedItemId);

        Task<ServiceResponse<IEnumerable<AttachmentResponseDto>>> GetItemAttachments(int donatedItemId);
        Task<ServiceResponse<IEnumerable<AttachmentResponseDto>>> GetRecipientAttachments(int donatedItemId);
        Task<ServiceResponse<IEnumerable<AttachmentResponseDto>>> GetAllAttachments(int donatedItemId);
        Task<ServiceResponse<AttachmentResponseDto>> GetAttachmentById(int attachmentId);
        Task<ServiceResponse<AttachmentResponseDto>> CreateAttachment(CreateAttachmentDto attachment);
        //Task<ServiceResponse<AttachmentResponseDto>> UpdateAttachment(UpdateAttachmentDto attachment);
        Task<ServiceResponse<bool>> DeleteAttachment(int attachmentId);

        Task<ServiceResponse<DonatedItemResponseDto>> UpdateItemAvailability(int itemId, bool isAvailable);
        Task<ServiceResponse<DonatedItemResponseDto>> MarkItemAsAvailable(int itemId);
        Task<ServiceResponse<DonatedItemResponseDto>> MarkItemAsUnavailable(int itemId);
        Task<ServiceResponse<DonatedItemResponseDto>> UpdateItemCategory(int itemId, ItemCategory category);

        Task<ServiceResponse<int>> GetTotalDonatedItemsCount();
        Task<ServiceResponse<int>> GetAvailableDonatedItemsCount();
        Task<ServiceResponse<int>> GetDonatedItemsCountByOrganization(int organizationId);
        Task<ServiceResponse<int>> GetDonatedItemsCountByDonor(string donorId);
        Task<ServiceResponse<int>> GetDonatedItemsCountByCategory(ItemCategory category);
        Task<ServiceResponse<Dictionary<ItemCategory, int>>> GetDonatedItemsCountToAllCategories();
        Task<ServiceResponse<Dictionary<string, int>>> GetTopDonors(int limit = 10);
        Task<ServiceResponse<Dictionary<int, int>>> GetTopOrganizationsByDonations(int limit = 10);

        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetRecentDonatedItems(int days = 30);
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsWithoutImages();
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsWithAttachments();
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsByDateRange(DateTime startDate, DateTime endDate);

        Task<ServiceResponse<int>> BulkUpdateItemCategories(ItemCategory oldCategory, ItemCategory newCategory);
        Task<ServiceResponse<int>> BulkMarkItemsAsUnavailable(int organizationId);
        Task<ServiceResponse<int>> DeleteOldDonatedItems(int daysOld = 365);

        //Task<ServiceResponse<bool>> DonatedItemExists(int id);
        //Task<ServiceResponse<bool>> IsDonatedItemAvailable(int id);
        //Task<ServiceResponse<bool>> HasDonatedItemImages(int id);
        //Task<ServiceResponse<bool>> HasDonatedItemAttachments(int id);
        //Task<ServiceResponse<bool>> IsDonor(string donorId);

        Task<ServiceResponse<DonatedItemResponseDto>> GetDonatedItemWithImages(int id);
        Task<ServiceResponse<DonatedItemResponseDto>> GetDonatedItemWithAttachments(int id);
        Task<ServiceResponse<DonatedItemResponseDto>> GetDonatedItemWithDonorAndOrganization(int id);

        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetMostRecentDonatedItems(int limit = 10);
        Task<ServiceResponse<Dictionary<DateTime, int>>> GetDonatedItemsTrend(int days = 30);

        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonorDonatedItemsHistory(string donorId);
        Task<ServiceResponse<int>> GetDonorTotalDonatedItemsCount(string donorId);
        Task<ServiceResponse<ItemCategory>> GetDonorMostCommonCategory(string donorId);

        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetOrganizationInventory(int organizationId);
        Task<ServiceResponse<int>> GetOrganizationAvailableItemsCount(int organizationId);
        Task<ServiceResponse<Dictionary<ItemCategory, int>>> GetOrganizationInventoryByCategory(int organizationId);

        Task<ServiceResponse<long>> GetTotalFileSize(int donatedItemId);
        Task<ServiceResponse<long>> GetTotalStorageUsed();
        Task<ServiceResponse<IEnumerable<AttachmentResponseDto>>> GetLargeAttachments(long sizeThreshold = 10485760); // 10MB default

        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetItemsByMultipleCategories(IEnumerable<ItemCategory> categories);
        Task<ServiceResponse<Dictionary<ItemCategory, decimal>>> GetCategoryDistributionPercentage();
        Task<ServiceResponse<IEnumerable<ItemCategory>>> GetMostPopularCategories(int limit = 2);

        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> SearchAvailableItemsByCategory(string searchTerm, ItemCategory category);
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetItemsByOrganizationAndCategory(int organizationId, ItemCategory category);
        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetItemsByDonorAndDateRange(string donorId, DateTime startDate, DateTime endDate);

        Task<ServiceResponse<DonatedItemResponseDto>> TransferItemToOrganization(int itemId, int newOrganizationId);
        Task<ServiceResponse<DonatedItemResponseDto>> UpdateItemDonor(int itemId, string newDonorId);

        Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetRecentlyUpdatedItems(int hours = 24);
        Task<ServiceResponse<Dictionary<string, int>>> GetActivityByDonor(int days = 30);
    }
}
