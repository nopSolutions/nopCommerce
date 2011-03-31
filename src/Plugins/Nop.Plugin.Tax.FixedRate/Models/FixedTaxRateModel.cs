using System.Collections.Generic;
using System.ComponentModel;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;

namespace Nop.Plugin.Tax.FixedRate.Models
{
    public class FixedTaxRateModel
    {
        public int TaxCategoryId { get; set; }

        [NopResourceDisplayName("Tax category")]
        public string TaxCategoryName { get; set; }

        [NopResourceDisplayName("Rate")]
        public decimal Rate { get; set; }
    }
}