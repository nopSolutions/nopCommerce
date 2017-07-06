using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Affiliates
{
    public partial class AffiliateListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Affiliates.List.SearchFirstName")]
        public string SearchFirstName { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.List.SearchLastName")]
        public string SearchLastName { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.List.SearchFriendlyUrlName")]
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