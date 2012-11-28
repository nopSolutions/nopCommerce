using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Settings;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    [Validator(typeof(OrderSettingsValidator))]
    public partial class OrderSettingsModel : BaseNopModel
    {
        public OrderSettingsModel()
        {
            GiftCards_Activated_OrderStatuses = new List<SelectListItem>();
            GiftCards_Deactivated_OrderStatuses = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Configuration.Settings.Order.IsReOrderAllowed")]
        public bool IsReOrderAllowed { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.MinOrderSubtotalAmount")]
        public decimal MinOrderSubtotalAmount { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.MinOrderTotalAmount")]
        public decimal MinOrderTotalAmount { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.AnonymousCheckoutAllowed")]
        public bool AnonymousCheckoutAllowed { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.TermsOfServiceEnabled")]
        public bool TermsOfServiceEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.OnePageCheckoutEnabled")]
        public bool OnePageCheckoutEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestsEnabled")]
        public bool ReturnRequestsEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestReasons")]
        public string ReturnRequestReasonsParsed { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestActions")]
        public string ReturnRequestActionsParsed { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.NumberOfDaysReturnRequestAvailable")]
        public int NumberOfDaysReturnRequestAvailable { get; set; }
        
        
        [NopResourceDisplayName("Admin.Configuration.Settings.Order.GiftCards_Activated")]
        public int GiftCards_Activated_OrderStatusId { get; set; }
        public IList<SelectListItem> GiftCards_Activated_OrderStatuses { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Order.GiftCards_Deactivated")]
        public int GiftCards_Deactivated_OrderStatusId { get; set; }
        public IList<SelectListItem> GiftCards_Deactivated_OrderStatuses { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
    }
}