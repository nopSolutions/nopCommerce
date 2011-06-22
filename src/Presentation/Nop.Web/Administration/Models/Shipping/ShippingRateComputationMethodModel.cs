using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Localization;
using Nop.Services.Shipping;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Shipping
{
    public class ShippingRateComputationMethodModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.IsActive")]
        public bool IsActive { get; set; }
        





        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}