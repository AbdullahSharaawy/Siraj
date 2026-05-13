using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.ItemImageDTOs
{
    public class ItemImageResponseDto
    {
        public int Id { get; set; }
        public string Path { get; set; } = null!;
        public int DonatedItemId { get; set; }
        public bool IsMain { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
