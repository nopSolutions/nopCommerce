using System.Collections.Generic;
using System.ComponentModel;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;

namespace Nop.Plugin.Tax.FixedRate.Models
{
    public class FixedTaxRateModel
    {
        public int TaxCategoryId { get; set; }

        [DisplayNameAttribute("Tax category")]
        public string TaxCategoryName { get; set; }

        [DisplayNameAttribute("Rate")]
        public decimal Rate { get; set; }
    }
}