using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Shipping.ByWeight.Models
{
    public class ShippingByWeightListModel : BaseNopModel
    {
        public ShippingByWeightListModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableShippingMethods = new List<SelectListItem>();
            Records = new List<ShippingByWeightModel>();
        }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.Country")]
        public int AddCountryId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.ShippingMethod")]
        public int AddShippingMethodId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.From")]
        public decimal AddFrom { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.To")]
        public decimal AddTo { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.UsePercentage")]
        public bool AddUsePercentage { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.ShippingChargePercentage")]
        public decimal AddShippingChargePercentage { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.ShippingChargeAmount")]
        public decimal AddShippingChargeAmount { get; set; }



        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated")]
        public bool LimitMethodsToCreated { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
        public string BaseWeightIn { get; set; }


        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableShippingMethods { get; set; }

        public IList<ShippingByWeightModel> Records { get; set; }
        
    }
}