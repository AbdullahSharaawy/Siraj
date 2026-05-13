using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheCharityDAL.Entities;

namespace TheCharityDAL.Database
{
    public class TheCharityDbContext : IdentityDbContext<User>
    {
        public TheCharityDbContext(DbContextOptions<TheCharityDbContext> options) : base(options)
        { }
        public TheCharityDbContext() { }

        
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<DonatedItem> DonatedItems { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<ItemImage> ItemImages { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationContactMethod> OrganizationContactMethods { get; set; }
        public DbSet<PaymentInfo> PaymentsInfo { get; set; }
        public DbSet<SharedCampaign> SharedCampaigns { get; set; }
        public DbSet<SoloCampaign> SoloCampaigns { get; set; }
       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. Campaign TPH configuration
            builder.Entity<Campaign>()
                .ToTable("Campaigns")
                .HasDiscriminator<string>("CampaignType")
                .HasValue<SharedCampaign>("Shared")
                .HasValue<SoloCampaign>("Solo");

            // 2. SoloCampaign relationship
            builder.Entity<SoloCampaign>()
                .HasOne(sc => sc.Organization)
                .WithMany(o => o.SoloCampaigns)
                .HasForeignKey(sc => sc.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. SharedCampaign many-to-many
            builder.Entity<SharedCampaign>()
                .HasMany(sc => sc.Organizations)
                .WithMany(o => o.SharedCampaigns)
                .UsingEntity(j => j.ToTable("SharedCampaignOrganizations"));

            // 4. DonatedItem - Attachment relationships (FIX THE ISSUE HERE)
            builder.Entity<DonatedItem>()
                .HasMany(di => di.ItemAttachments)
                .WithOne(a => a.DonatedItem)
                .HasForeignKey(a => a.DonatedItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DonatedItem>()
                .HasMany(di => di.RecipientAttachments)
                .WithOne() // No inverse navigation for RecipientAttachments
                .HasForeignKey(a => a.DonatedItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. Soft delete query filters
            builder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            builder.Entity<Campaign>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<Organization>().HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
