using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Messages
{
    public partial class NewsLetterSubscriptionListModel : BaseNopModel
    {
        public NewsLetterSubscriptionListModel()
        {
            AvailableStores = new List<SelectListItem>();
            ActiveList = new List<SelectListItem>();
            AvailableCustomerRoles = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchStore")]
        public int StoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive")]
        public int ActiveId { get; set; }
        [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.SearchActive")]
        public IList<SelectListItem> ActiveList { get; set; }

        [NopResourceDisplayName("Admin.Promotions.NewsLetterSubscriptions.List.CustomerRoles")]
        public int CustomerRoleId { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    }
}