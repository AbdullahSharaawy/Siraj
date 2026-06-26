using Microsoft.AspNetCore.Authorization;

namespace TheCharityBLL.Authorization.Attributes
{
    /// <summary>
    /// Allows access to users who can create campaigns for the specified organization.
    /// This includes SuperAdmin, OrganizationAdmin, and SubAdmin.
    /// </summary>
    public class CanCreateCampaignAttribute : AuthorizeAttribute
    {
        public CanCreateCampaignAttribute()
        {
            Policy = "CanCreateCampaign";
        }
    }
}
