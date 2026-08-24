using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheCharityDAL.Entities
{
    public class Attachment
    {
        public int Id { get; private set; }
        public string? Name { get; private set; }
        public string? Path { get; private set; }
        public long? FileSize { get; private set; }
        public string? ContentType { get; private set; }
        public bool IsItemAttachment { get; private set; } = true;
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }
        public DateTime? RegistrationDate { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }
        public int? DonatedItemId { get; private set; }

        [ForeignKey(nameof(DonatedItemId))]
        public DonatedItem? DonatedItem { get; private set; }
        public Attachment(int? donatedItemId, string? name, string? path, long? fileSize, string? contentType, bool isItemAttachment = true) { 
            this.DonatedItemId = donatedItemId;
            this.Name = name;
            this.Path = path;
            this.FileSize = fileSize;
            this.ContentType = contentType;
            this.IsItemAttachment = isItemAttachment;
        }
        public void EditName(string? name) {
            if (!string.IsNullOrEmpty(name)) { 
                this.Name = name;
                this.UpdatedOn = DateTime.Now;
            }
        }
        public void Delete() { 
            this.IsDeleted = true;
            this.DeletedOn = DateTime.Now;
        }
    }
}
