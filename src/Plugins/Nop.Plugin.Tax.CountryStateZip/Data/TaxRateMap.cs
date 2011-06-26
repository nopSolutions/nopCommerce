using System.Data.Entity.ModelConfiguration;
using Nop.Plugin.Tax.CountryStateZip.Domain;

namespace Nop.Plugin.Tax.CountryStateZip.Data
{
    public partial class TaxRateMap : EntityTypeConfiguration<TaxRate>
    {
        public TaxRateMap()
        {
            this.ToTable("TaxRate");
            this.HasKey(bp => bp.Id);
        }
    }
}