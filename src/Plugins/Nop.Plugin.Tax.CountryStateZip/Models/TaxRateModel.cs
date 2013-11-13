using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Tax.CountryStateZip.Models
{
    public class TaxRateModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Tax.CountryStateZip.Fields.Store")]
        public int StoreId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.CountryStateZip.Fields.Store")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.CountryStateZip.Fields.TaxCategory")]
        public int TaxCategoryId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.CountryStateZip.Fields.TaxCategory")]
        public string TaxCategoryName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.CountryStateZip.Fields.Country")]
        public int CountryId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.CountryStateZip.Fields.Country")]
        public string CountryName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.CountryStateZip.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.CountryStateZip.Fields.StateProvince")]
        public string StateProvinceName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.CountryStateZip.Fields.Zip")]
        public string Zip { get; set; }

        [NopResourceDisplayName("Plugins.Tax.CountryStateZip.Fields.Percentage")]
        public decimal Percentage { get; set; }
    }
}