using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs.AttachmentDTOs;
using TheCharityBLL.DTOs.DonatedItemDTOs;
using TheCharityBLL.DTOs.ItemImageDTOs;
using TheCharityBLL.DTOs.OrganizationDTOs;
using TheCharityBLL.DTOs.PaymentInfoDTOs;
using TheCharityBLL.Helpers;
using TheCharityBLL.Mapper;
using TheCharityBLL.Services.Abstraction;
using TheCharityBLL.ViewModels;
using TheCharityDAL.Entities;
using TheCharityDAL.Enums;
using TheCharityDAL.Repositories.Abstraction;

namespace TheCharityBLL.Services.Implementation
{
    public class DonatedItemService : IDonatedItemService
    {
        private readonly IDonatedItemsRepository _repository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly DontedItemMapper _mapper;
        public DonatedItemService(IDonatedItemsRepository repository, IOrganizationRepository organizationRepository)
        {
            _repository = repository;
            _mapper = new DontedItemMapper();
            _organizationRepository = organizationRepository;
        }

        public async Task<ServiceResponse<AttachmentResponseDto>> CreateAttachment(CreateAttachmentDto attachment)
        {
            if (!await _repository.DonatedItemExistsAsync(attachment.DonatedItemId))
            {
                return new ServiceResponse<AttachmentResponseDto>
                {
                    Success = false,
                    Message = $"Donated item with id {attachment.DonatedItemId} not found.",
                };
            }
            var attachmentName = string.Empty;
            try
            {
                if (attachment.FileUrl != null && attachment.FileUrl.Length > 0)
                {
                    var fileName = FileManager.UploadFile("DonatedItems/Attachments", attachment.FileUrl);
                    if (!fileName.StartsWith("/"))
                    {
                        return new ServiceResponse<AttachmentResponseDto>
                        {
                            Success = false,
                            Message = fileName,
                        };
                    }
                    attachmentName = fileName;
                    attachment.Path = fileName;
                    //attachment.FileSize= attachment.fileName.Length;
                }
                var attachmentEntity = _mapper.MapToAttachment(attachment);
                var createAttachment = await _repository.AddAttachmentAsync(attachmentEntity);
                var attachmentDto = _mapper.MapToAttachmentResponseDto(createAttachment);
                return new ServiceResponse<AttachmentResponseDto>
                {
                    Success = true,
                    Message = "Attachment added successfully.",
                    Data = attachmentDto
                };
            }
            catch
            {
                if (!string.IsNullOrEmpty(attachmentName))
                {
                    FileManager.RemoveFile("DonatedItems/Attachments", attachmentName);
                }
                return new ServiceResponse<AttachmentResponseDto>
                {
                    Success = false,
                    Message = "Failed to upload attachment"
                };
            }
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> CreateDonatedItem(CreateDonatedItemDto donatedItem)
        {
            if (!await _repository.IsDonorAsync(donatedItem.DonorId))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Donor with ID {donatedItem.DonorId} not found.",
                };
            }
            if (!await _organizationRepository.OrganizationExistsAsync(donatedItem.OrganizationId))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Organization with ID {donatedItem.OrganizationId} not found."
                };
            }
            var uploadedImageNames = new List<string>();
            var uploadAttachmentNames = new List<string>();
            try
            {
                var donatedItemEntity = _mapper.MapToDonatedItem(donatedItem);
                if (donatedItem.ImageFiles != null && donatedItem.ImageFiles.Any())
                {
                    for (var i = 0; i < donatedItem.ImageFiles.Count; i++)
                    {
                        var fileName = FileManager.UploadFile("DonatedItems/Images", donatedItem.ImageFiles[i]);
                        if (!fileName.StartsWith("/"))
                        {
                            foreach (var img in uploadedImageNames)
                            {
                                if (!string.IsNullOrEmpty(img))
                                {
                                    FileManager.RemoveFile("DonatedItems/Images", img);
                                }
                            }
                            return new ServiceResponse<DonatedItemResponseDto>
                            {
                                Success = false,
                                Message = fileName
                            };
                        }
                        uploadedImageNames.Add(fileName);
                        var itemImage = new ItemImage(fileName, null);
                        donatedItemEntity.AddImage(itemImage);
                    }
                }
                if (donatedItem.AttachmentFiles != null && donatedItem.AttachmentFiles.Any())
                {
                    foreach (var attachment in donatedItem.AttachmentFiles)
                    {
                        var fileName = FileManager.UploadFile("DonatedItems/Attachments", attachment);
                        if (!fileName.StartsWith("/"))
                        {
                            foreach (var attach in uploadAttachmentNames)
                            {
                                if (!string.IsNullOrEmpty(attach))
                                {
                                    FileManager.RemoveFile("DonatedItems/Attachments", attach);
                                }
                            }
                            foreach (var img in uploadedImageNames)
                            {
                                if (!string.IsNullOrEmpty(img))
                                {
                                    FileManager.RemoveFile("DonatedItems/Images", img);
                                }
                            }
                            return new ServiceResponse<DonatedItemResponseDto>
                            {
                                Success = false,
                                Message = fileName
                            };
                        }
                        var itemAttachment = new TheCharityDAL.Entities.Attachment(
                       donatedItemId: null,
                       name: Path.GetFileName(fileName),
                       path: fileName,
                       fileSize: attachment.Length,
                       contentType: attachment.ContentType,
                       isItemAttachment: true
               );
                        uploadAttachmentNames.Add(fileName);
                        donatedItemEntity.AddItemAttachment(itemAttachment);
                    }
                }

                var createDonated = await _repository.AddDonatedItemAsync(donatedItemEntity);
                var donatedDto = _mapper.MapToDonatedItemResponseDto(createDonated);
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = true,
                    Data = donatedDto,
                    Message = "Donated Item with images and attachments added successfully."
                };
            }
            catch
            {
                foreach (var attach in uploadAttachmentNames)
                {
                    if (!string.IsNullOrEmpty(attach))
                    {
                        FileManager.RemoveFile("DonatedItems/Attachments", attach);
                    }
                }
                foreach (var img in uploadedImageNames)
                {
                    if (!string.IsNullOrEmpty(img))
                    {
                        FileManager.RemoveFile("DonatedItems/Images", img);
                    }
                }
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = "File to create Donated Item"
                };
            }

        }

        public async Task<ServiceResponse<ItemImageResponseDto>> CreateItemImage(CreateItemImageDto itemImage)
        {
            if (!await _repository.DonatedItemExistsAsync(itemImage.DonatedItemId))
            {
                return new ServiceResponse<ItemImageResponseDto>
                {
                    Success = false,
                    Message = $"Donated item with id {itemImage.DonatedItemId} not found.",
                };
            }
            var imageName = string.Empty;
            try
            {
                if (itemImage.ImageUrl != null && itemImage.ImageUrl.Length > 0)
                {
                    var fileName = FileManager.UploadFile("DonatedItems/Images", itemImage.ImageUrl);
                    if (!fileName.StartsWith("/"))
                    {
                        return new ServiceResponse<ItemImageResponseDto>
                        {
                            Success = false,
                            Message = fileName
                        };
                    }
                    imageName = fileName;
                    itemImage.Path = fileName;

                }
                var itemImageEntity = _mapper.MapToItemImage(itemImage);
                var createItemImage = await _repository.AddItemImageAsync(itemImageEntity);
                var itemImageDto = _mapper.MapToItemImageResponseDto(createItemImage);
                return new ServiceResponse<ItemImageResponseDto>
                {
                    Success = true,
                    Message = "Item Image added successfully.",
                    Data = itemImageDto
                };
            }
            catch
            {
                if (!string.IsNullOrEmpty(imageName))
                {
                    FileManager.RemoveFile("DonatedItems/Images", imageName);
                }
                return new ServiceResponse<ItemImageResponseDto>
                {
                    Success = false,
                    Message = "Failed to upload image"
                };
            }
        }

        public async Task<ServiceResponse<int>> BulkMarkItemsAsUnavailable(int organizationId)
        {
            if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Organization with ID {organizationId} not found."
                };
            }

            var updatedCount = await _repository.BulkMarkItemsAsUnavailableAsync(organizationId);

            return new ServiceResponse<int>
            {
                Success = true,
                Data = updatedCount,
                Message = $"{updatedCount} items marked as unavailable."
            };
        }

        public async Task<ServiceResponse<int>> BulkUpdateItemCategories(ItemCategory oldCategory, ItemCategory newCategory)
        {
            var updatedCount = await _repository.BulkUpdateItemCategoriesAsync(oldCategory, newCategory);

            return new ServiceResponse<int>
            {
                Success = true,
                Data = updatedCount,
                Message = $"{updatedCount} items updated from {oldCategory} to {newCategory}."
            };
        }

        public async Task<ServiceResponse<bool>> DeleteAttachment(int attachmentId)
        {
            var attachment = await _repository.GetAttachmentByIdAsync(attachmentId);
            if (attachment == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Attachment with id {attachmentId} not found.",
                };
            }
            FileManager.RemoveFile("DonatedItems/Attachments", attachment.Path);
            await _repository.DeleteAttachmentAsync(attachmentId);
            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Attachment deleted successfully.",
            };
        }

        public async Task<ServiceResponse<bool>> DeleteDonatedItem(int id)
        {
            if (!await _repository.DonatedItemExistsAsync(id))
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Donated Item with ID {id} not found.",
                };
            }
            //soft delete=>so we dont delete images and attachments
            await _repository.DeleteDonatedItemAsync(id);
            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Donated Item deleted successfully."
            };
        }

        public async Task<ServiceResponse<bool>> DeleteItemImage(int imageId)
        {
            var itemImage = await _repository.GetItemImageByIdAsync(imageId);
            if (itemImage == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Item image with ID {imageId} not found.",
                };
            }
            FileManager.RemoveFile("DonatedItems/Images", itemImage.Path);

            await _repository.DeleteItemImageAsync(imageId);
            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Image deleted successfully.",
            };
        }

        public async Task<ServiceResponse<int>> DeleteOldDonatedItems(int daysOld = 365)
        {
            if (daysOld < 0)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Enter valid day"
                };
            }
            var deletedCount = await _repository.DeleteOldDonatedItemsAsync(daysOld);
            return new ServiceResponse<int>
            {
                Success = true,
                Data = deletedCount,
                Message = $"Old {daysOld} Donated Items deleted successfully."
            };
        }

        public async Task<ServiceResponse<Dictionary<string, int>>> GetActivityByDonor(int days = 30)
        {
            if (days < 0)
            {
                return new ServiceResponse<Dictionary<string, int>>
                {
                    Success = false,
                    Message = "Days must be greater than zero."
                };
            }

            var result = await _repository.GetActivityByDonorAsync(days);

            return new ServiceResponse<Dictionary<string, int>>
            {
                Success = true,
                Data = result,
                Message = "Donor activity retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<AttachmentResponseDto>>> GetAllAttachments(int donatedItemId)
        {
            if (await _repository.DonatedItemExistsAsync(donatedItemId) == false)
            {
                return new ServiceResponse<IEnumerable<AttachmentResponseDto>>
                {
                    Success = false,
                    Message = $"Donated item with id {donatedItemId} not found.",
                };
            }
            var attachments = await _repository.GetAllAttachmentsAsync(donatedItemId);
            var attachmentsDto = _mapper.MapToAttachmentResponseDtos(attachments);
            return new ServiceResponse<IEnumerable<AttachmentResponseDto>>
            {
                Success = true,
                Message = "Attachments retrieved successfully.",
                Data = attachmentsDto
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetAllDonatedItems(bool includeDeleted = false)
        {
            var donatedItems = await _repository.GetAllDonatedItemsAsync(includeDeleted);
            var donatedItemsDtos = _mapper.MapToDonatedItemResponseDtos(donatedItems);
            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = donatedItemsDtos,
                Message = "Donated Items retrieved successfully."
            };
        }

        public async Task<ServiceResponse<AttachmentResponseDto>> GetAttachmentById(int attachmentId)
        {
            var attachment = await _repository.GetAttachmentByIdAsync(attachmentId);
            if (attachment == null)
            {
                return new ServiceResponse<AttachmentResponseDto>
                {
                    Success = false,
                    Message = $"Attachment with id {attachmentId} not found.",
                };
            }
            var attachmentDto = _mapper.MapToAttachmentResponseDto(attachment);
            return new ServiceResponse<AttachmentResponseDto>
            {
                Success = true,
                Message = "Attachment retrieved successfully.",
                Data = attachmentDto
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetAvailableDonatedItems()
        {
            var availableItems = await _repository.GetAvailableDonatedItemsAsync();
            if (!availableItems.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = $"No Donated Items is available now."
                };
            }
            var donatedItemDtos = _mapper.MapToDonatedItemResponseDtos(availableItems);
            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = donatedItemDtos,
                Message = "Available donated items retrieved successfully."
            };
        }

        public async Task<ServiceResponse<int>> GetAvailableDonatedItemsCount()
        {
            var availableCount = await _repository.GetAvailableDonatedItemsCountAsync();
            return new ServiceResponse<int>
            {
                Success = true,
                Data = availableCount,
                Message = $"Avaliable Donated item count is {availableCount}"
            };
        }

        public async Task<ServiceResponse<Dictionary<ItemCategory, decimal>>> GetCategoryDistributionPercentage()
        {
            var result = await _repository.GetCategoryDistributionPercentageAsync();

            return new ServiceResponse<Dictionary<ItemCategory, decimal>>
            {
                Success = true,
                Data = result,
                Message = "Category distribution percentage retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDeletedDonatedItems()
        {
            var deletedItems = await _repository.GetDeletedDonatedItemsAsync();
            if (!deletedItems.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = $"No deleted donated items avaliable"
                };
            }
            var donatedItemDtos = _mapper.MapToDonatedItemResponseDtos(deletedItems);
            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = donatedItemDtos,
                Message = "Deleted donated items retrieved successfully."
            };
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> GetDonatedItemById(int id)
        {
            var donatedItem = await _repository.GetDonatedItemByIdAsync(id);
            if (donatedItem == null)
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Donated Item with ID {id} not found.",
                };
            }
            var donatedItemDto = _mapper.MapToDonatedItemResponseDto(donatedItem);
            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = donatedItemDto,
                Message = "Donated Item retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsByCategory(ItemCategory category)
        {
            var itemsByCategory = await _repository.GetDonatedItemsByCategoryAsync(category);
            if (!itemsByCategory.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = $"No donated items available for this category: {category}"
                };
            }
            var donatedItemDtos = _mapper.MapToDonatedItemResponseDtos(itemsByCategory);
            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = donatedItemDtos,
                Message = $"Donated items in category '{category}' retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = "Start date must be before end date."
                };
            }
            var itemsByDateRange = await _repository.GetDonatedItemsByDateRangeAsync(startDate, endDate);
            if (!itemsByDateRange.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = $"No donated items registered between {startDate.ToShortDateString()} and {endDate.ToShortDateString()}"
                };
            }
            var donatedItemDtos = _mapper.MapToDonatedItemResponseDtos(itemsByDateRange);
            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = donatedItemDtos,
                Message = $"Donated items registered between {startDate.ToShortDateString()} and {endDate.ToShortDateString()} retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsByDonor(string donorId)
        {
            if (!await _repository.IsDonorAsync(donorId))
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Data = null,
                    Message = $"Donor with ID {donorId} not found.",
                };
            }
            var itemsByDonor = await _repository.GetDonatedItemsByDonorAsync(donorId);
            if (!itemsByDonor.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = $"No donated items registered for Donor Id {donorId}"
                };
            }
            var donatedItemDtos = _mapper.MapToDonatedItemResponseDtos(itemsByDonor);
            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = donatedItemDtos,
                Message = $"Donated items by donor with ID {donorId} retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsByOrganization(int organizationId)
        {
            if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Data = null,
                    Message = $"Organization with ID {organizationId} not found.",
                };
            }
            var donatedItemsByOrganization = await _repository.GetDonatedItemsByOrganizationAsync(organizationId);
            if (!donatedItemsByOrganization.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = $"No donated items registered for Organization Id {organizationId}"
                };
            }
            var donatedItemDtos = _mapper.MapToDonatedItemResponseDtos(donatedItemsByOrganization);
            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = donatedItemDtos,
                Message = $"Donated items for organization with ID {organizationId} retrieved successfully."
            };
        }

        public async Task<ServiceResponse<int>> GetDonatedItemsCountByCategory(ItemCategory category)
        {
            var count = await _repository.GetDonatedItemsCountByCategoryAsync(category);
            return new ServiceResponse<int>
            {
                Success = true,
                Data = count,
                Message = $"Donated items count for category {category} retrieved successfully."
            };
        }

        public async Task<ServiceResponse<int>> GetDonatedItemsCountByDonor(string donorId)
        {
            if (!await _repository.IsDonorAsync(donorId))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Donor with ID {donorId} not found.",
                };
            }
            var count = await _repository.GetDonatedItemsCountByDonorAsync(donorId);
            return new ServiceResponse<int>
            {
                Success = true,
                Data = count,
                Message = "Total donated items count retrieved successfully."
            };
        }

        public async Task<ServiceResponse<int>> GetDonatedItemsCountByOrganization(int organizationId)
        {
            if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Organization with ID {organizationId} does not exist."
                };
            }
            var count = await _repository.GetDonatedItemsCountByOrganizationAsync(organizationId);
            return new ServiceResponse<int>
            {
                Success = true,
                Data = count,
                Message = $"Donated items count for organization ID {organizationId} retrieved successfully."
            };
        }

        public async Task<ServiceResponse<Dictionary<ItemCategory, int>>> GetDonatedItemsCountToAllCategories()
        {
            var result = await _repository.GetDonatedItemsCountToAllCategoriesAsync();

            return new ServiceResponse<Dictionary<ItemCategory, int>>
            {
                Success = true,
                Data = result,
                Message = "Donated items count per category retrieved successfully."
            };
        }

        public async Task<ServiceResponse<Dictionary<DateTime, int>>> GetDonatedItemsTrend(int days = 30)
        {
            if (days <= 0)
            {
                return new ServiceResponse<Dictionary<DateTime, int>>
                {
                    Success = false,
                    Message = "Days must be greater than zero."
                };
            }

            var result = await _repository.GetDonatedItemsTrendAsync(days);

            return new ServiceResponse<Dictionary<DateTime, int>>
            {
                Success = true,
                Data = result,
                Message = "Donated items trend retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsWithAttachments()
        {
            var itemsWithAttachments = await _repository.GetDonatedItemsWithAttachmentsAsync();
            var donatedItemDtos = _mapper.MapToDonatedItemResponseDtos(itemsWithAttachments);
            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = donatedItemDtos,
                Message = "Donated items with attachments retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonatedItemsWithoutImages()
        {
            var donatedItemsWithoutImages = await _repository.GetDonatedItemsWithoutImagesAsync();
            var donatedItemDtos = _mapper.MapToDonatedItemResponseDtos(donatedItemsWithoutImages);
            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = donatedItemDtos,
                Message = "Donated items without images retrieved successfully."
            };
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> GetDonatedItemWithAttachments(int id)
        {
            var donatedItem = await _repository.GetDonatedItemWithAttachmentsAsync(id);
            if (donatedItem == null)
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Donated Item with ID {id} not found.",
                };
            }
            var donatedDto = _mapper.MapToDonatedItemResponseDto(donatedItem);
            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = donatedDto,
                Message = "Donated Item with attachment retrieved successfully."
            };
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> GetDonatedItemWithDonorAndOrganization(int id)
        {
            var donatedItem = await _repository.GetDonatedItemWithDonorAndOrganizationAsync(id);
            if (donatedItem == null)
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Donated Item with ID {id} not found.",
                };
            }
            var donatedDto = _mapper.MapToDonatedItemResponseDto(donatedItem);
            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = donatedDto,
                Message = "Donated Item with donor and organization retrieved successfully."
            };
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> GetDonatedItemWithImages(int id)
        {
            var donatedItem = await _repository.GetDonatedItemWithImagesAsync(id);
            if (donatedItem == null)
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Donated Item with ID {id} not found.",
                };
            }
            var donatedDto = _mapper.MapToDonatedItemResponseDto(donatedItem);
            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = donatedDto,
                Message = "Donated Item with images retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetDonorDonatedItemsHistory(string donorId)
        {
            if (!await _repository.IsDonorAsync(donorId))
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Data = null,
                    Message = $"Donor with ID {donorId} not found.",
                };
            }
            var donatedItems = await _repository.GetDonorDonatedItemsHistoryAsync(donorId);
            if (!donatedItems.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = "No donated items found for this donor."
                };
            }
            var donatedDto = _mapper.MapToDonatedItemResponseDtos(donatedItems);
            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = donatedDto,
                Message = "Donated items history retrieved successfully."
            };
        }

        public async Task<ServiceResponse<ItemCategory>> GetDonorMostCommonCategory(string donorId)
        {
            if (!await _repository.IsDonorAsync(donorId))
            {
                return new ServiceResponse<ItemCategory>
                {
                    Success = false,
                    Message = $"Donor with ID {donorId} not found."
                };
            }

            var category = await _repository.GetDonorMostCommonCategoryAsync(donorId);

            return new ServiceResponse<ItemCategory>
            {
                Success = true,
                Data = category,
                Message = "Most common donated category retrieved successfully."
            };
        }

        public async Task<ServiceResponse<int>> GetDonorTotalDonatedItemsCount(string donorId)
        {
            if (!await _repository.IsDonorAsync(donorId))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Donor with ID {donorId} not found."
                };
            }

            var count = await _repository.GetDonorTotalDonatedItemsCountAsync(donorId);

            return new ServiceResponse<int>
            {
                Success = true,
                Data = count,
                Message = $"Donor total donated items count is {count}."
            };
        }

        public async Task<ServiceResponse<IEnumerable<AttachmentResponseDto>>> GetItemAttachments(int donatedItemId)
        {
            if (!await _repository.DonatedItemExistsAsync(donatedItemId))
            {
                return new ServiceResponse<IEnumerable<AttachmentResponseDto>>
                {
                    Success = false,
                    Message = $"Donated item with id {donatedItemId} not found."
                };
            }

            var attachments = await _repository.GetItemAttachmentsAsync(donatedItemId);
            var attachmentsDto = _mapper.MapToAttachmentResponseDtos(attachments);

            return new ServiceResponse<IEnumerable<AttachmentResponseDto>>
            {
                Success = true,
                Data = attachmentsDto,
                Message = "Item attachments retrieved successfully."
            };
        }

        public async Task<ServiceResponse<ItemImageResponseDto>> GetItemImageById(int imageId)
        {
            var image = await _repository.GetItemImageByIdAsync(imageId);

            if (image == null)
            {
                return new ServiceResponse<ItemImageResponseDto>
                {
                    Success = false,
                    Message = $"Item image with ID {imageId} not found."
                };
            }

            var dto = _mapper.MapToItemImageResponseDto(image);

            return new ServiceResponse<ItemImageResponseDto>
            {
                Success = true,
                Data = dto,
                Message = "Item image retrieved successfully."
            };
        }

        public async Task<ServiceResponse<int>> GetItemImageCountByDonatedItem(int donatedItemId)
        {
            if (!await _repository.DonatedItemExistsAsync(donatedItemId))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Donated item with id {donatedItemId} not found."
                };
            }

            var count = await _repository.GetItemImageCountAsync(donatedItemId);

            return new ServiceResponse<int>
            {
                Success = true,
                Data = count,
                Message = $"Item images count is {count}."
            };
        }

        public async Task<ServiceResponse<IEnumerable<ItemImageResponseDto>>> GetItemImages(int donatedItemId)
        {
            if (!await _repository.DonatedItemExistsAsync(donatedItemId))
            {
                return new ServiceResponse<IEnumerable<ItemImageResponseDto>>
                {
                    Success = false,
                    Message = $"Donated item with id {donatedItemId} not found."
                };
            }

            var images = await _repository.GetItemImagesAsync(donatedItemId);

            var dto = _mapper.MapToItemImageResponseDtos(images);

            return new ServiceResponse<IEnumerable<ItemImageResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Item images retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetItemsByDonorAndDateRange(string donorId, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = "Start date must be before end date."
                };
            }
            if (!await _repository.IsDonorAsync(donorId))
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = $"Donor with ID {donorId} not found."
                };
            }

            var items = await _repository.GetItemsByDonorAndDateRangeAsync(donorId, startDate, endDate);

            if (!items.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = "No donated items found in this date range."
                };
            }

            var dto = _mapper.MapToDonatedItemResponseDtos(items);

            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Donated items retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetItemsByMultipleCategories(IEnumerable<ItemCategory> categories)
        {
            if (!categories.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = "Categories list cannot be empty."
                };
            }

            var items = await _repository.GetItemsByMultipleCategoriesAsync(categories);

            var dto = _mapper.MapToDonatedItemResponseDtos(items);

            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Items retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetItemsByOrganizationAndCategory(int organizationId, ItemCategory category)
        {
            if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = $"Organization with ID {organizationId} not found."
                };
            }

            var items = await _repository.GetItemsByOrganizationAndCategoryAsync(organizationId, category);
            if (!items.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = "No items found for this organization and category."
                };
            }

            var dto = _mapper.MapToDonatedItemResponseDtos(items);

            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Items retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<AttachmentResponseDto>>> GetLargeAttachments(long sizeThreshold = 10485760)
        {
            var attachments = await _repository.GetLargeAttachmentsAsync(sizeThreshold);

            var dto = _mapper.MapToAttachmentResponseDtos(attachments);

            return new ServiceResponse<IEnumerable<AttachmentResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Large attachments retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<ItemCategory>>> GetMostPopularCategories(int limit = 2)
        {
            if (limit < 0)
            {
                return new ServiceResponse<IEnumerable<ItemCategory>>
                {
                    Success = false,
                    Message = "Limit must be greater than zero."
                };
            }
            var result = await _repository.GetMostPopularCategoriesAsync(limit);

            return new ServiceResponse<IEnumerable<ItemCategory>>
            {
                Success = true,
                Data = result,
                Message = "Most popular categories retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetMostRecentDonatedItems(int limit = 10)
        {
            if (limit < 0)
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = "Limit must be greater than zero."
                };
            }
            var items = await _repository.GetMostRecentDonatedItemsAsync(limit);
            if (!items.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = ""
                };
            }
            var dto = _mapper.MapToDonatedItemResponseDtos(items);

            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Most recent donated items retrieved successfully."
            };
        }

        public async Task<ServiceResponse<int>> GetOrganizationAvailableItemsCount(int organizationId)
        {
            if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Organization with ID {organizationId} not found."
                };
            }

            var count = await _repository.GetOrganizationAvailableItemsCountAsync(organizationId);

            return new ServiceResponse<int>
            {
                Success = true,
                Data = count,
                Message = $"Available items count is {count}."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetOrganizationInventory(int organizationId)
        {
            if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = $"Organization with ID {organizationId} not found."
                };
            }

            var items = await _repository.GetOrganizationInventoryAsync(organizationId);

            var dto = _mapper.MapToDonatedItemResponseDtos(items);

            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Organization inventory retrieved successfully."
            };
        }

        public async Task<ServiceResponse<Dictionary<ItemCategory, int>>> GetOrganizationInventoryByCategory(int organizationId)
        {
            if (!await _organizationRepository.OrganizationExistsAsync(organizationId))
            {
                return new ServiceResponse<Dictionary<ItemCategory, int>>
                {
                    Success = false,
                    Message = $"Organization with ID {organizationId} not found."
                };
            }

            var result = await _repository.GetOrganizationInventoryByCategoryAsync(organizationId);

            return new ServiceResponse<Dictionary<ItemCategory, int>>
            {
                Success = true,
                Data = result,
                Message = "Organization inventory by category retrieved successfully."
            };
        }

        public async Task<ServiceResponse<ItemImageResponseDto>> GetPrimaryItemImageForDonatedItem(int donatedItemId)
        {
            if (!await _repository.DonatedItemExistsAsync(donatedItemId))
            {
                return new ServiceResponse<ItemImageResponseDto>
                {
                    Success = false,
                    Message = $"Donated item with id {donatedItemId} not found."
                };
            }

            var image = await _repository.GetPrimaryItemImageAsync(donatedItemId);

            if (image == null)
            {
                return new ServiceResponse<ItemImageResponseDto>
                {
                    Success = false,
                    Message = "Primary image not found."
                };
            }

            var dto = _mapper.MapToItemImageResponseDto(image);

            return new ServiceResponse<ItemImageResponseDto>
            {
                Success = true,
                Data = dto,
                Message = "Primary item image retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetRecentDonatedItems(int days = 30)
        {
            if (days < 0)
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = "Days must be greater than or equal to zero."
                };
            }
            var items = await _repository.GetRecentDonatedItemsAsync(days);
            if (!items.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = "No recent donated items found."
                };
            }

            var dto = _mapper.MapToDonatedItemResponseDtos(items);

            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Recent donated items retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetRecentlyUpdatedItems(int hours = 24)
        {
            if (hours < 0)
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = "Hours must be greater than or equal to zero."
                };
            }
            var items = await _repository.GetRecentlyUpdatedItemsAsync(hours);
            if (!items.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = "No recently updated items found."
                };
            }
            var dto = _mapper.MapToDonatedItemResponseDtos(items);

            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Recently updated items retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<AttachmentResponseDto>>> GetRecipientAttachments(int donatedItemId)
        {
            if (!await _repository.DonatedItemExistsAsync(donatedItemId))
            {
                return new ServiceResponse<IEnumerable<AttachmentResponseDto>>
                {
                    Success = false,
                    Message = $"Donated item with id {donatedItemId} not found."
                };
            }

            var attachments = await _repository.GetRecipientAttachmentsAsync(donatedItemId);
            if (!attachments.Any())
            {
                return new ServiceResponse<IEnumerable<AttachmentResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<AttachmentResponseDto>(),
                    Message = "No attachments found for this donated item."
                };
            }

            var dto = _mapper.MapToAttachmentResponseDtos(attachments);

            return new ServiceResponse<IEnumerable<AttachmentResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Recipient attachments retrieved successfully."
            };
        }

        public async Task<ServiceResponse<Dictionary<string, int>>> GetTopDonors(int limit = 10)
        {
            if (limit < 0)
            {
                return new ServiceResponse<Dictionary<string, int>>
                {
                    Success = false,
                    Message = "Limit must be greater than or equal to zero."
                };
            }
            var result = await _repository.GetTopDonorsAsync(limit);

            return new ServiceResponse<Dictionary<string, int>>
            {
                Success = true,
                Data = result,
                Message = "Top donors retrieved successfully."
            };
        }

        public async Task<ServiceResponse<Dictionary<int, int>>> GetTopOrganizationsByDonations(int limit = 10)
        {
            if (limit < 0)
            {
                return new ServiceResponse<Dictionary<int, int>>
                {
                    Success = false,
                    Message = "Limit must be greater than or equal to zero."
                };
            }
            var result = await _repository.GetTopOrganizationsByDonationsAsync(limit);

            return new ServiceResponse<Dictionary<int, int>>
            {
                Success = true,
                Data = result,
                Message = "Top donors retrieved successfully."
            };
        }

        public async Task<ServiceResponse<int>> GetTotalDonatedItemsCount()
        {
            var count = await _repository.GetTotalDonatedItemsCountAsync();

            return new ServiceResponse<int>
            {
                Success = true,
                Data = count,
                Message = $"Total donated items count is {count}."
            };
        }

        public async Task<ServiceResponse<long>> GetTotalFileSize(int donatedItemId)
        {
            if (!await _repository.DonatedItemExistsAsync(donatedItemId))
            {
                return new ServiceResponse<long>
                {
                    Success = false,
                    Message = $"Donated item with id {donatedItemId} not found."
                };
            }

            var size = await _repository.GetTotalFileSizeAsync(donatedItemId);

            return new ServiceResponse<long>
            {
                Success = true,
                Data = size,
                Message = $"Total file size is {size} bytes."
            };
        }

        public async Task<ServiceResponse<long>> GetTotalStorageUsed()
        {
            var size = await _repository.GetTotalStorageUsedAsync();

            return new ServiceResponse<long>
            {
                Success = true,
                Data = size,
                Message = $"Total storage used is {size} bytes."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> GetUnavailableDonatedItems()
        {
            var items = await _repository.GetUnavailableDonatedItemsAsync();
            if (!items.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = "No unavailable donated items found."
                };
            }
            var dto = _mapper.MapToDonatedItemResponseDtos(items);

            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Unavailable donated items retrieved successfully."
            };
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> MarkItemAsAvailable(int itemId)
        {
            if (!await _repository.DonatedItemExistsAsync(itemId))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Donated item with ID {itemId} not found."
                };
            }

            var result = await _repository.MarkItemAsAvailableAsync(itemId);
            var donatedItem = _mapper.MapToDonatedItemResponseDto(result);
            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = donatedItem,
                Message = "Item marked as available."
            };
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> MarkItemAsUnavailable(int itemId)
        {
            if (!await _repository.DonatedItemExistsAsync(itemId))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Donated item with ID {itemId} not found."
                };
            }

            var result = await _repository.MarkItemAsUnavailableAsync(itemId);

            var dto = _mapper.MapToDonatedItemResponseDto(result);

            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = dto,
                Message = "Item marked as unavailable."
            };
        }

        public async Task<ServiceResponse<bool>> RestoreDonatedItem(int id)
        {
            if (!await _repository.DonatedItemExistsAsync(id))
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Donated item with ID {id} not found."
                };
            }

            await _repository.RestoreDonatedItemAsync(id);

            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Donated item restored successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> SearchAvailableItemsByCategory(string searchTerm, ItemCategory category)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = "Search term cannot be empty."
                };
            }
            var items = await _repository.SearchAvailableItemsByCategoryAsync(searchTerm, category);
            if (!items.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = "No matching items found."
                };
            }

            var dto = _mapper.MapToDonatedItemResponseDtos(items);

            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Search results retrieved successfully."
            };
        }

        public async Task<ServiceResponse<IEnumerable<DonatedItemResponseDto>>> SearchDonatedItems(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = false,
                    Message = "Search term cannot be empty."
                };
            }
            var items = await _repository.SearchDonatedItemsAsync(searchTerm);
            if (!items.Any())
            {
                return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<DonatedItemResponseDto>(),
                    Message = "No matching donated items found."
                };
            }
            var dto = _mapper.MapToDonatedItemResponseDtos(items);

            return new ServiceResponse<IEnumerable<DonatedItemResponseDto>>
            {
                Success = true,
                Data = dto,
                Message = "Search results retrieved successfully."
            };
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> TransferItemToOrganization(int itemId, int newOrganizationId)
        {
            if (!await _repository.DonatedItemExistsAsync(itemId))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Item with ID {itemId} not found."
                };
            }

            if (!await _organizationRepository.OrganizationExistsAsync(newOrganizationId))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Organization with ID {newOrganizationId} not found."
                };
            }

            var item = await _repository.TransferItemToOrganizationAsync(itemId, newOrganizationId);

            var dto = _mapper.MapToDonatedItemResponseDto(item);

            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = dto,
                Message = "Item transferred successfully."
            };
        }

        //public async Task<ServiceResponse<AttachmentResponseDto>> UpdateAttachment(UpdateAttachmentDto attachment)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ServiceResponse<DonatedItemResponseDto>> UpdateDonatedItem(int id,UpdateDonatedItemDto donatedItem)
        {
            if (!await _organizationRepository.OrganizationExistsAsync(donatedItem.OrganizationId.Value))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = "Organization not found."
                };
            }
            var existingItem = await _repository.GetDonatedItemByIdAsync(id);
            if (existingItem == null)
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Donated Item with ID {id} not found.",
                };
            }
            existingItem.EditAvailability(donatedItem.IsAvailable ?? existingItem.IsAvailable);
            existingItem.EditDescription(donatedItem.Description ?? existingItem.Description);
            existingItem.EditItemCategory(donatedItem.ItemCategory ?? existingItem.ItemCategory);
            existingItem.EditName(donatedItem.Name ?? existingItem.Name);
            existingItem.EditOrganization(donatedItem.OrganizationId ?? existingItem.OrganizationId);
            var update = await _repository.UpdateDonatedItemAsync(existingItem);
            var itemDto = _mapper.MapToDonatedItemResponseDto(update);
            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = itemDto,
                Message = "Donated Item updated successfully."
            };
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> UpdateItemAvailability(int itemId, bool isAvailable)
        {
            if (!await _repository.DonatedItemExistsAsync(itemId))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Item with ID {itemId} not found."
                };
            }

            var item = await _repository.UpdateItemAvailabilityAsync(itemId, isAvailable);

            var dto = _mapper.MapToDonatedItemResponseDto(item);

            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = dto,
                Message = "Item availability updated successfully."
            };
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> UpdateItemCategory(int itemId, ItemCategory category)
        {
            if (!await _repository.DonatedItemExistsAsync(itemId))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Item with ID {itemId} not found."
                };
            }

            var item = await _repository.UpdateItemCategoryAsync(itemId, category);

            var dto = _mapper.MapToDonatedItemResponseDto(item);

            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = dto,
                Message = "Item category updated successfully."
            };
        }

        public async Task<ServiceResponse<DonatedItemResponseDto>> UpdateItemDonor(int itemId, string newDonorId)
        {
            if (!await _repository.DonatedItemExistsAsync(itemId))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Item with ID {itemId} not found."
                };
            }

            if (!await _repository.IsDonorAsync(newDonorId))
            {
                return new ServiceResponse<DonatedItemResponseDto>
                {
                    Success = false,
                    Message = $"Donor with ID {newDonorId} not found."
                };
            }

            var item = await _repository.UpdateItemDonorAsync(itemId, newDonorId);

            var dto = _mapper.MapToDonatedItemResponseDto(item);

            return new ServiceResponse<DonatedItemResponseDto>
            {
                Success = true,
                Data = dto,
                Message = "Item donor updated successfully."
            };
        }

        //public async Task<ServiceResponse<ItemImageResponseDto>> UpdateItemImage(UpdateItemImageDto itemImage)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
