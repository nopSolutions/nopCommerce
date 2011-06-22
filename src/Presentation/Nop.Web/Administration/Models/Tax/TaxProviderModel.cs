using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Tax
{
    public class TaxProviderModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.Tax.Providers.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Tax.Providers.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Tax.Providers.Fields.IsPrimaryTaxProvider")]
        public bool IsPrimaryTaxProvider { get; set; }





        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}