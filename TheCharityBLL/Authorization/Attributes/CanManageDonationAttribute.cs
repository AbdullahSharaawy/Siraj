using Microsoft.AspNetCore.Authorization;

namespace TheCharityBLL.Authorization.Attributes
{
    public class CanManageDonationAttribute : AuthorizeAttribute
    {
        public CanManageDonationAttribute()
        {
            Policy = "CanManageDonation";
        }
    }
}