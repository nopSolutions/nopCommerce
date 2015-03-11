using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Affiliates
{
    public partial class AffiliateListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Affiliates.List.SearchFirstName")]
        [AllowHtml]
        public string SearchFirstName { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.List.SearchLastName")]
        [AllowHtml]
        public string SearchLastName { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.List.SearchFriendlyUrlName")]
        [AllowHtml]
        public string SearchFriendlyUrlName { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.List.LoadOnlyWithOrders")]
        public bool LoadOnlyWithOrders { get; set; }
        [NopResourceDisplayName("Admin.Affiliates.List.OrdersCreatedFromUtc")]
        [UIHint("DateNullable")]
        public DateTime? OrdersCreatedFromUtc { get; set; }
        [NopResourceDisplayName("Admin.Affiliates.List.OrdersCreatedToUtc")]
        [UIHint("DateNullable")]
        public DateTime? OrdersCreatedToUtc { get; set; }
    }
}