using Nop.Data.Mapping;
using Nop.Plugin.Tax.CountryStateZip.Domain;

namespace Nop.Plugin.Tax.CountryStateZip.Data
{
    public partial class TaxRateMap : NopEntityTypeConfiguration<TaxRate>
    {
        public TaxRateMap()
        {
            this.ToTable("TaxRate");
            this.HasKey(tr => tr.Id);
            this.Property(tr => tr.Percentage).HasPrecision(18, 4);
        }
    }
}