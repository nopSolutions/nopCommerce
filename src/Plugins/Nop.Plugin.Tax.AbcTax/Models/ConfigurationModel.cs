using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.AbcTax.Models
{
    public record ConfigurationModel : BaseSearchModel
    {
        public ConfigurationModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableTaxCategories = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.Store")]
        public int AddStoreId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.Country")]
        public int AddCountryId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.StateProvince")]
        public int AddStateProvinceId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.Zip")]
        public string AddZip { get; set; }
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.TaxCategory")]
        public int AddTaxCategoryId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.Percentage")]
        public decimal AddPercentage { get; set; }
        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.IsTaxJarEnabled")]
        public bool IsTaxJarEnabled { get; set; }

        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.TaxJarAPIToken")]
        public string TaxJarAPIToken { get; set; }

        [NopResourceDisplayName("Plugins.Tax.AbcTax.Fields.IsDebugMode")]
        public bool IsDebugMode { get; set; }

        public string TaxCategoriesCanNotLoadedError { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
        public IList<SelectListItem> AvailableTaxCategories { get; set; }
    }
}