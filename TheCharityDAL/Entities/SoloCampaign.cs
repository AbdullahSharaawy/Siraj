using System.ComponentModel.DataAnnotations.Schema;
using TheCharityDAL.Enums;

namespace TheCharityDAL.Entities
{
    public class SoloCampaign: Campaign
    {
        public Organization? Organization { get; private set; }
        public SoloCampaign(string? title, string? description, string? imgPath, int? target, int? achieved, CampaignStatus? status, CampaignType? type, int? organizationId, DateTime deadline) : base(title, description, imgPath, target, achieved, status, type, deadline)
        {
            this.OrganizationId = organizationId;
        }
        private SoloCampaign() { }
    }
}
