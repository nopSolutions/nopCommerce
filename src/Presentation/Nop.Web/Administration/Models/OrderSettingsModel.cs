using System.Web.Mvc;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework;

namespace Nop.Admin.Models
{
    public class OrderSettingsModel
    {
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


        public string PrimaryStoreCurrencyCode { get; set; }
    }
}