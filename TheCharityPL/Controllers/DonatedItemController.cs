using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using TheCharityBLL.DTOs;
using TheCharityBLL.DTOs.AttachmentDTOs;
using TheCharityBLL.DTOs.CampaignDTOs;
using TheCharityBLL.DTOs.DonatedItemDTOs;
using TheCharityBLL.DTOs.ItemImageDTOs;
using TheCharityBLL.Services.Abstraction;

using TheCharityDAL.Enums;

namespace TheCharityPL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonatedItemController : ControllerBase
    {
        private readonly IDonatedItemService _donatedItemService;
        public DonatedItemController(IDonatedItemService donatedItemService)
        {
            _donatedItemService = donatedItemService;
        }
        //donated items
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var result = await _donatedItemService.GetAllDonatedItems(includeDeleted);
            
            return Ok(result);
        }

        [HttpGet("{itemId:int}")]
        public async Task<IActionResult> GetById(int itemId)
        {
            var result = await _donatedItemService.GetDonatedItemById(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDonatedItemDto dto)
        {
            var result = await _donatedItemService.CreateDonatedItem(dto);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);

        }

        [HttpPut("{itemId:int}")]
        public async Task<IActionResult> Update(int itemId, [FromBody] UpdateDonatedItemDto dto)
        {
            var result = await _donatedItemService.UpdateDonatedItem(itemId,dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{itemId:int}")]
        public async Task<IActionResult> Delete(int itemId)
        {
            var result = await _donatedItemService.DeleteDonatedItem(itemId);
            return result.Success ? NoContent() : NotFound(result);
        }

        [HttpPatch("{itemId:int}/restore")]
        public async Task<IActionResult> Restore(int itemId)
        {
            var result = await _donatedItemService.RestoreDonatedItem(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("filter/organization/{organizationId:int}")]
        public async Task<IActionResult> GetByOrganization(int organizationId)
        {
            var result = await _donatedItemService.GetDonatedItemsByOrganization(organizationId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("filter/donor/{donorId}")]
        public async Task<IActionResult> GetByDonor(string donorId)
        {
            var result = await _donatedItemService.GetDonatedItemsByDonor(donorId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("filter/category")]
        public async Task<IActionResult> GetByCategory([FromQuery] ItemCategory category)
        {
            var result = await _donatedItemService.GetDonatedItemsByCategory(category);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("filter/available")]
        public async Task<IActionResult> GetAvailable()
        {
            var result = await _donatedItemService.GetAvailableDonatedItems();
            return Ok(result);
        }

        [HttpGet("filter/unavailable")]
        public async Task<IActionResult> GetUnAvailable()
        {
            var result = await _donatedItemService.GetUnavailableDonatedItems();
            return Ok(result);
        }

        [HttpGet("filter/deleted")]
        public async Task<IActionResult> GetDeleted()
        {
            var result = await _donatedItemService.GetDeletedDonatedItems();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            var result = await _donatedItemService.SearchDonatedItems(searchTerm);
            return result.Success ? Ok(result) : NotFound(result);
        }


        //images
        [HttpGet("image/{itemId:int}")]
        public async Task<IActionResult> GetItemImages(int itemId)
        {
            var result = await _donatedItemService.GetItemImages(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{imageId:int}/image")]
        public async Task<IActionResult> GetImageById(int imageId)
        {
            var result = await _donatedItemService.GetItemImageById(imageId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("image")]
        public async Task<IActionResult> CreateImage([FromForm] CreateItemImageDto dto)
        {
            var result = await _donatedItemService.CreateItemImage(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("image/{imageId:int}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var result = await _donatedItemService.DeleteItemImage(imageId);
            return result.Success ? NoContent() : NotFound(result);
        }

        [HttpGet("image/{itemId:int}/count")]
        public async Task<IActionResult> GetImageCount(int itemId)
        {
            var result = await _donatedItemService.GetItemImageCountByDonatedItem(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("image/{itemId:int}/primary")]
        public async Task<IActionResult> GetPrimaryImage(int itemId)
        {
            var result = await _donatedItemService.GetPrimaryItemImageForDonatedItem(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }


        //attachment
        [HttpGet("attachment/{itemId:int}/all")]
        public async Task<IActionResult> GetAllAttachments(int itemId)
        {
            var result = await _donatedItemService.GetAllAttachments(itemId);
            return Ok(result);
        }
        [HttpGet("attachment/{itemId:int}")]
        public async Task<IActionResult> GetItemAttachments(int itemId)
        {
            var result = await _donatedItemService.GetItemAttachments(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("attachment/{itemId:int}/recipient")]
        public async Task<IActionResult> GetItemRecipientAttachments(int itemId)
        {
            var result = await _donatedItemService.GetRecipientAttachments(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{attachmentId:int}/attachment")]
        public async Task<IActionResult> GetAttachmentById(int attachmentId)
        {
            var result = await _donatedItemService.GetAttachmentById(attachmentId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("attachment")]
        public async Task<IActionResult> CreateAttachment([FromForm] CreateAttachmentDto dto)
        {
            var result = await _donatedItemService.CreateAttachment(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("attachment/{id:int}")]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            var result = await _donatedItemService.DeleteAttachment(id);
            return result.Success ? NoContent() : NotFound(result);
        }


        //advanced
        [HttpPatch("{itemId:int}/availability")]
        public async Task<IActionResult> UpdateAvailability(int itemId, [FromQuery] bool isAvailable)
        {
            var result = await _donatedItemService.UpdateItemAvailability(itemId, isAvailable);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPatch("{itemId:int}/available")]
        public async Task<IActionResult> MarkAsAvailable(int itemId)
        {
            var result = await _donatedItemService.MarkItemAsAvailable(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPatch("{itemId:int}/unavailable")]
        public async Task<IActionResult> MarkAsUnAvailable(int itemId)
        {
            var result = await _donatedItemService.MarkItemAsUnavailable(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPatch("{itemId:int}/category")]
        public async Task<IActionResult> UpdateCategory(int itemId, [FromQuery] ItemCategory category)
        {
            var result = await _donatedItemService.UpdateItemCategory(itemId, category);
            return result.Success ? Ok(result) : NotFound(result);
        }

        //statistics
        [HttpGet("count")]
        public async Task<IActionResult> GetTotalItemsCount()
        {
            var result = await _donatedItemService.GetTotalDonatedItemsCount();
            return Ok(result);
        }

        [HttpGet("availabl/count")]
        public async Task<IActionResult> GetAvailablItemsCount()
        {
            var result = await _donatedItemService.GetAvailableDonatedItemsCount();
            return Ok(result);
        }

        [HttpGet("organization/{organizationId:int}/count")]
        public async Task<IActionResult> GetItemsCountByOrganization(int organizationId)
        {
            var result = await _donatedItemService.GetDonatedItemsCountByOrganization(organizationId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("donor/{donorId}/count")]
        public async Task<IActionResult> GetItemsCountByDonor(string donorId)
        {
            var result = await _donatedItemService.GetDonatedItemsCountByDonor(donorId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("category/count")]
        public async Task<IActionResult> GetItemsCountByCategory(ItemCategory category)
        {
            var result = await _donatedItemService.GetDonatedItemsCountByCategory(category);
            return Ok(result);
        }

        [HttpGet("categories/count/all")]
        public async Task<IActionResult> GetItemsCountToAllCategories()
        {
            var result = await _donatedItemService.GetDonatedItemsCountToAllCategories();
            return Ok(result);
        }

        [HttpGet("top-donors")]
        public async Task<IActionResult> GetTopDonors(int top)
        {
            var result = await _donatedItemService.GetTopDonors(top);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("top-organization/donation")]
        public async Task<IActionResult> GetTopOrganizations(int top)
        {
            var result = await _donatedItemService.GetTopOrganizationsByDonations(top);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        // advanced queries
        [HttpGet("items/recent")]
        public async Task<IActionResult> GetRecentItems([FromQuery] int days = 30)
        {
            var result = await _donatedItemService.GetRecentDonatedItems(days);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("items/without-images")]
        public async Task<IActionResult> GetItemsWithoutImages()
        {
            var result = await _donatedItemService.GetDonatedItemsWithoutImages();
            return Ok(result);
        }

        [HttpGet("items/with-attachments")]
        public async Task<IActionResult> GetItemsWithAttachments()
        {
            var result = await _donatedItemService.GetDonatedItemsWithAttachments();
            return Ok(result);
        }

        [HttpGet("items/date-range")]
        public async Task<IActionResult> GetItemsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _donatedItemService.GetDonatedItemsByDateRange(startDate, endDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPatch("bulk/category")]
        public async Task<IActionResult> BulkUpdateItemCategories([FromQuery] ItemCategory oldCategory, [FromQuery] ItemCategory newCategory)
        {
            var result = await _donatedItemService.BulkUpdateItemCategories(oldCategory, newCategory);
            return Ok(result);
        }

        [HttpPatch("bulk/unavailable")]
        public async Task<IActionResult> BulkMarkItemsAsUnavailable([FromQuery] int organizationId)
        {
            var result = await _donatedItemService.BulkMarkItemsAsUnavailable(organizationId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("delete-old")]
        public async Task<IActionResult> DeleteOldItems([FromQuery] int daysOld = 365)
        {
            var result = await _donatedItemService.DeleteOldDonatedItems(daysOld);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{itemId:int}/with-images")]
        public async Task<IActionResult> GetItemWithImages(int itemId)
        {
            var result = await _donatedItemService.GetDonatedItemWithImages(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{itemId:int}/with-attachments")]
        public async Task<IActionResult> GetItemWithAttachments(int itemId)
        {
            var result = await _donatedItemService.GetDonatedItemWithAttachments(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{itemId:int}/details")]
        public async Task<IActionResult> GetItemWithDonorAndOrganization(int itemId)
        {
            var result = await _donatedItemService.GetDonatedItemWithDonorAndOrganization(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("recent/limit")]
        public async Task<IActionResult> GetMostRecentItems([FromQuery] int limit = 10)
        {
            var result = await _donatedItemService.GetMostRecentDonatedItems(limit);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("trend")]
        public async Task<IActionResult> GetItemsTrend([FromQuery] int days = 30)
        {
            var result = await _donatedItemService.GetDonatedItemsTrend(days);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("donor/{donorId}/history")]
        public async Task<IActionResult> GetDonorDonatedItemsHistory(string donorId)
        {
            var result = await _donatedItemService.GetDonorDonatedItemsHistory(donorId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("donor/count/{donorId}")]
        public async Task<IActionResult> GetDonorTotalItemsCount(string donorId)
        {
            var result = await _donatedItemService.GetDonorTotalDonatedItemsCount(donorId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("donor/{donorId}/favorite-category")]
        public async Task<IActionResult> GetDonorMostCommonCategory(string donorId)
        {
            var result = await _donatedItemService.GetDonorMostCommonCategory(donorId);
            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpGet("organization/{organizationId}/inventory")]
        public async Task<IActionResult> GetOrganizationInventory(int organizationId)
        {
            var result = await _donatedItemService.GetOrganizationInventory(organizationId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("organization/{organizationId}/available-count")]
        public async Task<IActionResult> GetOrganizationAvailableItemsCount(int organizationId)
        {
            var result = await _donatedItemService.GetOrganizationAvailableItemsCount(organizationId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("organization/{organizationId}/inventory-by-category")]
        public async Task<IActionResult> GetOrganizationInventoryByCategory(int organizationId)
        {
            var result = await _donatedItemService.GetOrganizationInventoryByCategory(organizationId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{itemId:int}/storage")]
        public async Task<IActionResult> GetTotalFileSize(int itemId)
        {
            var result = await _donatedItemService.GetTotalFileSize(itemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("storage/total")]
        public async Task<IActionResult> GetTotalStorageUsed()
        {
            var result = await _donatedItemService.GetTotalStorageUsed();
            return Ok(result);
        }

        [HttpGet("attachments/large")]
        public async Task<IActionResult> GetLargeAttachments([FromQuery] long sizeThreshold)
        {
            var result = await _donatedItemService.GetLargeAttachments(sizeThreshold);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("categories/multiple")]
        public async Task<IActionResult> GetItemsByMultipleCategories([FromBody] IEnumerable<ItemCategory> categories)
        {
            var result = await _donatedItemService.GetItemsByMultipleCategories(categories);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("categories/distribution")]
        public async Task<IActionResult> GetCategoryDistributionPercentage()
        {
            var result = await _donatedItemService.GetCategoryDistributionPercentage();
            return Ok(result);
        }

        [HttpGet("categories/popular")]
        public async Task<IActionResult> GetMostPopularCategories([FromQuery] int limit = 2)
        {
            var result = await _donatedItemService.GetMostPopularCategories(limit);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("search/category")]
        public async Task<IActionResult> SearchAvailableItemsByCategory([FromQuery] string searchTerm, [FromQuery] ItemCategory category)
        {
            var result = await _donatedItemService.SearchAvailableItemsByCategory(searchTerm, category);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("organization/{organizationId}/category")]
        public async Task<IActionResult> GetItemsByOrganizationAndCategory(int organizationId, [FromQuery] ItemCategory category)
        {
            var result = await _donatedItemService.GetItemsByOrganizationAndCategory(organizationId, category);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("donor/{donorId}/date-range")]
        public async Task<IActionResult> GetItemsByDonorAndDateRange(string donorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _donatedItemService.GetItemsByDonorAndDateRange(donorId, startDate, endDate);
            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpPatch("{itemId:int}/transfer")]
        public async Task<IActionResult> TransferItemToOrganization(int itemId, [FromQuery] int newOrganizationId)
        {
            var result = await _donatedItemService.TransferItemToOrganization(itemId, newOrganizationId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPatch("{itemId:int}/donor")]
        public async Task<IActionResult> UpdateItemDonor(int itemId, [FromQuery] string newDonorId)
        {
            var result = await _donatedItemService.UpdateItemDonor(itemId, newDonorId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("recently-updated")]
        public async Task<IActionResult> GetRecentlyUpdatedItems([FromQuery] int hours = 24)
        {
            var result = await _donatedItemService.GetRecentlyUpdatedItems(hours);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("activity/donors")]
        public async Task<IActionResult> GetActivityByDonor([FromQuery] int days = 30)
        {
            var result = await _donatedItemService.GetActivityByDonor(days);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
