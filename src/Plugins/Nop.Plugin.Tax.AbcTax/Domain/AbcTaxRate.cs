using Nop.Core;

namespace Nop.Plugin.Tax.AbcTax.Domain
{
    public partial class AbcTaxRate : BaseEntity
    {
        public int StoreId { get; set; }
        public int TaxCategoryId { get; set; }
        public int CountryId { get; set; }
        public int StateProvinceId { get; set; }
        public string Zip { get; set; }
        public decimal Percentage { get; set; }
        public bool IsTaxJarEnabled { get; set; }
    }
}