using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Messages
{


    public class NewsLetterSubscriptionListModel : BaseNopModel
    {
        public GridModel<NewsLetterSubscriptionModel> NewsLetterSubscriptions { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
        public string SearchEmail { get; set; }
    }
}