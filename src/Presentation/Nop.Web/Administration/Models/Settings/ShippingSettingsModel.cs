using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Admin.Models.Common;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;

namespace Nop.Admin.Models.Settings
{
    public class ShippingSettingsModel
    {
        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXEnabled")]
        public bool FreeShippingOverXEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXValue")]
        public decimal FreeShippingOverXValue { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.EstimateShippingEnabled")]
        public bool EstimateShippingEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.ShippingOriginAddress")]
        public AddressModel ShippingOriginAddress { get; set; }
    }
}