using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Admin.Models
{
    public class NewsLetterSubscriptionModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.Fields.CreatedOnUTC")]
        public DateTime CreatedOnUTC { get; set; }
    }
}