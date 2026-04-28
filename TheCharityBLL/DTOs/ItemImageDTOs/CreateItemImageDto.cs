using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.ItemImageDTOs
{
    public class CreateItemImageDto
    {
        [Required(ErrorMessage = "Path is required.")]
        [MaxLength(1000, ErrorMessage = "Path cannot exceed 1000 characters.")]
        public string Path { get; set; } = null!;
        public IFormFile ImageUrl { get; set; } = null!;
        [Required(ErrorMessage = "DonatedItem id is required.")]
        public int DonatedItemId { get; set; }
    }
}
