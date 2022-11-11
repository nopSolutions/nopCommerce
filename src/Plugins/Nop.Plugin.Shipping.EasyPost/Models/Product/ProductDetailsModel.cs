using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.EasyPost.Models.Product
{
    /// <summary>
    /// Represents an additional product details model
    /// </summary>
    public record ProductDetailsModel : BaseNopModel
    {
        #region Ctor

        public ProductDetailsModel()
        {
            AvailablePredefinedPackages = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Product.Fields.PredefinedPackage")]
        public string EasyPostPredefinedPackage { get; set; }
        public List<SelectListItem> AvailablePredefinedPackages { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Product.Fields.HtsNumber")]
        public string EasyPostHtsNumber { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.EasyPost.Product.Fields.OriginCountry")]
        public string EasyPostOriginCountry { get; set; }
        public List<SelectListItem> AvailableCountries { get; set; }

        #endregion
    }
}