using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;

namespace Nop.Admin.Models
{
    public class ShippingSettingsModel
    {
        [NopResourceDisplayName("Admin.Configuration.Shipping.Settings.Fields.FreeShippingOverXEnabled")]
        public bool FreeShippingOverXEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Settings.Fields.FreeShippingOverXValue")]
        public decimal FreeShippingOverXValue { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Settings.Fields.EstimateShippingEnabled")]
        public bool EstimateShippingEnabled { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Shipping.Settings.Fields.ShippingOriginAddress")]
        public AddressModel ShippingOriginAddress { get; set; }
    }
}