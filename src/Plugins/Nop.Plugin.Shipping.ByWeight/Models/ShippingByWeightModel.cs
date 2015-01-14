using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Shipping.ByWeight.Models
{
    public class ShippingByWeightModel : BaseNopEntityModel
    {
        public ShippingByWeightModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableShippingMethods = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.Store")]
        public int StoreId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.Store")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.Warehouse")]
        public int WarehouseId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.Warehouse")]
        public string WarehouseName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.Country")]
        public int CountryId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.Country")]
        public string CountryName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.StateProvince")]
        public string StateProvinceName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.Zip")]
        public string Zip { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.ShippingMethod")]
        public int ShippingMethodId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.ShippingMethod")]
        public string ShippingMethodName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.From")]
        public decimal From { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.To")]
        public decimal To { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.AdditionalFixedCost")]
        public decimal AdditionalFixedCost { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.PercentageRateOfSubtotal")]
        public decimal PercentageRateOfSubtotal { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.RatePerWeightUnit")]
        public decimal RatePerWeightUnit { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.LowerWeightLimit")]
        public decimal LowerWeightLimit { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.ByWeight.Fields.DataHtml")]
        public string DataHtml { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
        public string BaseWeightIn { get; set; }


        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
        public IList<SelectListItem> AvailableShippingMethods { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
    }
}