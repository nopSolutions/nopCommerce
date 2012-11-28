using Nop.Admin.Models.Common;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public partial class ShippingSettingsModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXEnabled")]
        public bool FreeShippingOverXEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXValue")]
        public decimal FreeShippingOverXValue { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXIncludingTax")]
        public bool FreeShippingOverXIncludingTax { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.EstimateShippingEnabled")]
        public bool EstimateShippingEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToCustomers")]
        public bool DisplayShipmentEventsToCustomers { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.ShippingOriginAddress")]
        public AddressModel ShippingOriginAddress { get; set; }
    }
}