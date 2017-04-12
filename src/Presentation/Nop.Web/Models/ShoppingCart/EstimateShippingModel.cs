using System.Collections.Generic;
#if NET451
using System.Web.Mvc;
#endif
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.ShoppingCart
{
    public partial class EstimateShippingModel : BaseNopModel
    {
        public EstimateShippingModel()
        {
#if NET451
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
#endif
        }

        public bool Enabled { get; set; }

        [NopResourceDisplayName("ShoppingCart.EstimateShipping.Country")]
        public int? CountryId { get; set; }
        [NopResourceDisplayName("ShoppingCart.EstimateShipping.StateProvince")]
        public int? StateProvinceId { get; set; }
        [NopResourceDisplayName("ShoppingCart.EstimateShipping.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

#if NET451
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
#endif
    }

    public partial class EstimateShippingResultModel : BaseNopModel
    {
        public EstimateShippingResultModel()
        {
            ShippingOptions = new List<ShippingOptionModel>();
            Warnings = new List<string>();
        }

        public IList<ShippingOptionModel> ShippingOptions { get; set; }

        public IList<string> Warnings { get; set; }

        #region Nested Classes

        public partial class ShippingOptionModel : BaseNopModel
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public string Price { get; set; }
        }

        #endregion
    }
}