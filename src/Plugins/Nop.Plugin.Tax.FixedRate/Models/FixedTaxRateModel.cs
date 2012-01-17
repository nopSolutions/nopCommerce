using Nop.Web.Framework;

namespace Nop.Plugin.Tax.FixedRate.Models
{
    public class FixedTaxRateModel
    {
        public int TaxCategoryId { get; set; }

        [NopResourceDisplayName("Plugins.Tax.FixedRate.Fields.TaxCategoryName")]
        public string TaxCategoryName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.FixedRate.Fields.Rate")]
        public decimal Rate { get; set; }
    }
}