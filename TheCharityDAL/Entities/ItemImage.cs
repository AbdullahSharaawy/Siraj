using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheCharityDAL.Entities
{
    public class ItemImage
    {
        public int Id { get; private set; }
        public string? Path { get; private set; }
        public int? DonatedItemId { get; private set; }

        [ForeignKey(nameof(DonatedItemId))]
        public DonatedItem? DonatedItem { get; private set; }
        // 
        public bool IsMain { get; set; } = false;
        //
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }
        public DateTime? RegistrationDate { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }
        public ItemImage(string? path, int? donatedItemId) {
            this.Path = path;
            this.DonatedItemId = donatedItemId;
        }
        public void EditImage(string? path) {
            if (!string.IsNullOrEmpty(path)) {
                this.Path = path;
                this.UpdatedOn = DateTime.Now;
            }
        }
        public void Delete() { 
            this.IsDeleted = true;
            this.DeletedOn = DateTime.Now;
        }
    }
}
