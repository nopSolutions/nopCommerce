using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.AbcTax.Models
{
    public record CountryStateZipModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.Store")]
        public int StoreId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.Store")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.TaxCategory")]
        public int TaxCategoryId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.TaxCategory")]
        public string TaxCategoryName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.Country")]
        public int CountryId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.Country")]
        public string CountryName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.StateProvince")]
        public string StateProvinceName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.Zip")]
        public string Zip { get; set; }

        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.Percentage")]
        public decimal Percentage { get; set; }

        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.IsTaxJarEnabled")]
        public bool IsTaxJarEnabled { get; set; }
    }
}