using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityDAL.Enums;

namespace TheCharityBLL.DTOs.DonatedItemDTOs
{
    public class CreateDonatedItemDto
    {
        [Required(ErrorMessage = "DonorId is required.")]
        public string DonorId { get; set; } = null!;

        [Required(ErrorMessage = "OrganizationId is required.")]
        public int OrganizationId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; set; } = null!;
        
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Item category is required.")]
        [EnumDataType(typeof(ItemCategory), ErrorMessage = "Invalid item category.")]
        public ItemCategory ItemCategory { get; set; }

        /////
        [MaxLength(5,ErrorMessage ="Can not upload more than 5 images")]
        public List<IFormFile>? ImageFiles { get; set; }
        [MaxLength(5, ErrorMessage = "Can not upload more than 5 attachment")]
        public List<IFormFile>? AttachmentFiles { get; set; }

    }

}
