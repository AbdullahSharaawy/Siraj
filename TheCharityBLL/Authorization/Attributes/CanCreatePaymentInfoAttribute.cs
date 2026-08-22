using Microsoft.AspNetCore.Mvc;
using TheCharityBLL.Authorization.Filters;

namespace TheCharityBLL.Authorization.Attributes
{
    public class CanCreatePaymentInfoAttribute : ServiceFilterAttribute
    {
        public CanCreatePaymentInfoAttribute()
            : base(typeof(CanCreatePaymentInfoFilter))
        {
        }
    }
}