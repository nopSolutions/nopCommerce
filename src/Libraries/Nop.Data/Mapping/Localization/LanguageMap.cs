using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Mapping.Localization
{
    public partial class LanguageMap : NopEntityTypeConfiguration<Language>
    {
        public override void Configure(EntityTypeBuilder<Language> builder)
        {
            base.Configure(builder);
            builder.ToTable("Language");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Name).IsRequired().HasMaxLength(100);
            builder.Property(l => l.LanguageCulture).IsRequired().HasMaxLength(20);
            builder.Property(l => l.UniqueSeoCode).HasMaxLength(2);
            builder.Property(l => l.FlagImageFileName).HasMaxLength(50);
        }
    }
}