using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Mapping.Localization
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class LocaleStringResourceMap : NopEntityTypeConfiguration<LocaleStringResource>
    {
        public override void Configure(EntityTypeBuilder<LocaleStringResource> builder)
        {
            base.Configure(builder);
            builder.ToTable("LocaleStringResource");
            builder.HasKey(lsr => lsr.Id);
            builder.Property(lsr => lsr.ResourceName).IsRequired().HasMaxLength(200);
            builder.Property(lsr => lsr.ResourceValue).IsRequired();


            builder.HasOne(lsr => lsr.Language)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(lsr => lsr.LanguageId);
        }
    }
}