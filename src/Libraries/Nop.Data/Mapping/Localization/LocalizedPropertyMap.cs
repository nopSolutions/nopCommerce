using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Mapping.Localization
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class LocalizedPropertyMap : NopEntityTypeConfiguration<LocalizedProperty>
    {
        public override void Configure(EntityTypeBuilder<LocalizedProperty> builder)
        {
            base.Configure(builder);
            builder.ToTable("LocalizedProperty");
            builder.HasKey(lp => lp.Id);

            builder.Property(lp => lp.LocaleKeyGroup).IsRequired().HasMaxLength(400);
            builder.Property(lp => lp.LocaleKey).IsRequired().HasMaxLength(400);
            builder.Property(lp => lp.LocaleValue).IsRequired();

            builder.HasOne(lp => lp.Language)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(lp => lp.LanguageId);
        }
    }
}