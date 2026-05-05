using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.AttachmentDTOs
{
    public class CreateAttachmentDto
    {
        [Required(ErrorMessage = "DonatedItem id is required.")]

        public int DonatedItemId { get; set; }

        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Path is required.")]
        [MaxLength(1000, ErrorMessage = "Path cannot exceed 1000 characters.")]
        public string? Path { get; set; }
        public IFormFile FileUrl { get; set; } = null!;

        [Range(1, long.MaxValue, ErrorMessage = "FileSize must be a positive value.")]
        public long? FileSize { get; set; }

        [MaxLength(100, ErrorMessage = "ContentType cannot exceed 100 characters.")]
        public string? ContentType { get; set; }

        [Required(ErrorMessage = "Is Item is required.")]

        public bool IsItemAttachment { get; set; } = true;
    }
}
