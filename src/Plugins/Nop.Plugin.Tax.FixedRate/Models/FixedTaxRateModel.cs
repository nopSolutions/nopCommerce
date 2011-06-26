using System.ComponentModel;

namespace Nop.Plugin.Tax.FixedRate.Models
{
    public class FixedTaxRateModel
    {
        public int TaxCategoryId { get; set; }

        [DisplayName("Tax category")]
        public string TaxCategoryName { get; set; }

        [DisplayNameAttribute("Rate")]
        public decimal Rate { get; set; }
    }
}