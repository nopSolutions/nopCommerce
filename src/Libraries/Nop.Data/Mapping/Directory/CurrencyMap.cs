using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CurrencyMap : NopEntityTypeConfiguration<Currency>
    {
        public override void Configure(EntityTypeBuilder<Currency> builder)
        {
            base.Configure(builder);
            builder.ToTable("Currency");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.Property(c => c.CurrencyCode).IsRequired().HasMaxLength(5);
            builder.Property(c => c.DisplayLocale).HasMaxLength(50);
            builder.Property(c => c.CustomFormatting).HasMaxLength(50);
            builder.Property(c => c.Rate);

            builder.Ignore(c => c.RoundingType);
        }
    }
}