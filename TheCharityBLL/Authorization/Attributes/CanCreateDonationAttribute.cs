using Microsoft.AspNetCore.Mvc;
using TheCharityBLL.Authorization.Filters;

namespace TheCharityBLL.Authorization.Attributes
{
    public class CanCreateDonationAttribute : ServiceFilterAttribute
    {
        public CanCreateDonationAttribute()
            : base(typeof(CanCreateDonationFilter))
        {
        }
    }
}