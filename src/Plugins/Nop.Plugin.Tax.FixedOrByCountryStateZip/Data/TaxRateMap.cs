using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Data
{
    public partial class TaxRateMap : NopEntityTypeConfiguration<TaxRate>
    {
        public override void Configure(EntityTypeBuilder<TaxRate> builder)
        {
            builder.ToTable("TaxRate");
            builder.HasKey(tr => tr.Id);
            builder.Property(tr => tr.Percentage);//.HasPrecision(18, 4);
            base.Configure(builder);
        }
    }
}