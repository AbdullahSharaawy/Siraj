using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations.Schema;
using TheCharityDAL.Enums;

namespace TheCharityDAL.Entities
{
    public class DonatedItem
    {
        public int Id { get; private set; }
        public string? Name { get; private set; }
        public string? Description { get; private set; } = string.Empty;
        public ItemCategory? ItemCategory { get; private set; }
        public bool? IsAvailable { get; private set; } = true;
        public int OrganizationId { get; private set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization? Organization { get; private set; }
        public string DonorId { get; private set; }

        [ForeignKey(nameof(DonorId))]
        public User? Donor { get; private set; }
        public virtual ICollection<ItemImage> Images { get; private set; } = new List<ItemImage>();
        public virtual ICollection<Attachment> ItemAttachments { get; private set; } = new List<Attachment>();
        public virtual ICollection<Attachment> RecipientAttachments { get; private set; } = new List<Attachment>();
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }
        public DateTime RegistrationDate { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; private set; }
        public DonatedItem(string donorId, int organizationId, string? name, string? description, ItemCategory? itemCategory)
        {
            this.Name = name;
            this.Description = description;
            this.ItemCategory = itemCategory;
            this.OrganizationId = organizationId;
            this.DonorId = donorId;
        }
        public void AddImage(ItemImage? image)
        {
            if (image != null)
            {
                this.Images?.Add(image);
                this.UpdatedOn = DateTime.UtcNow;
            }
        }
        public void RemoveImage(ItemImage image)
        {
            if (image != null)
            {
                this.Images?.Remove(image);
                this.UpdatedOn = DateTime.UtcNow;
            }
        }
        public void AddItemAttachment(Attachment attachment)
        {
            if (attachment != null)
            {
                this.ItemAttachments?.Add(attachment);
                this.UpdatedOn = DateTime.UtcNow;
            }
        }
        public void RemoveItemAttachment(Attachment attachment)
        {
            if (attachment != null)
            {
                this.ItemAttachments?.Remove(attachment);
            }
        }
        public void AddRecipientAttachment(Attachment attachment)
        {
            if (attachment != null)
            {
                this.RecipientAttachments?.Add(attachment);
                this.UpdatedOn = DateTime.UtcNow;
            }
        }
        public void RemoveRecipientAttachment(Attachment attachment)
        {
            if (attachment != null)
            {
                this.RecipientAttachments?.Remove(attachment);
                this.UpdatedOn = DateTime.UtcNow;
            }
        }
        public void EditName(string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                this.Name = name;
                UpdatedOn = DateTime.UtcNow;
            }
        }
        public void EditDescription(string? description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                this.Description = description;
                UpdatedOn = DateTime.UtcNow;
            }
        }
        public void EditItemCategory(ItemCategory? category)
        {
            if (category.HasValue)
            {
                this.ItemCategory = category;
                UpdatedOn = DateTime.UtcNow;
            }
        }
        public void EditAvailability(bool? isAvailable)
        {
            if (isAvailable.HasValue)
            {
                this.IsAvailable = isAvailable;
                UpdatedOn = DateTime.UtcNow;
            }
        }
        public void EditOrganization(int organizationId)
        {
            this.OrganizationId = organizationId;
            UpdatedOn = DateTime.UtcNow;
        }
        public void EditDonor(string donorId)
        {
            if (!string.IsNullOrEmpty(donorId))
            {
                this.DonorId = donorId;
                UpdatedOn = DateTime.UtcNow;
            }
        }
        public void Delete()
        {
            this.IsDeleted = true;
            this.DeletedOn = DateTime.UtcNow;
        }
        public void Restore()
        {
            this.IsDeleted = false;
            this.UpdatedOn = DateTime.UtcNow;
        }
    }
}
