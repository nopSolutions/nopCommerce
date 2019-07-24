using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Mapping.Localization
{
    /// <summary>
    /// Represents a localized property mapping configuration
    /// </summary>
    public partial class LocalizedPropertyMap : NopEntityTypeConfiguration<LocalizedProperty>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<LocalizedProperty> builder)
        {
            builder.ToTable(nameof(LocalizedProperty));
            builder.HasKey(property => property.Id);

            builder.Property(property => property.LocaleKeyGroup).HasMaxLength(400).IsRequired();
            builder.Property(property => property.LocaleKey).HasMaxLength(400).IsRequired();
            builder.Property(property => property.LocaleValue).IsRequired();

            builder.HasOne(property => property.Language)
                .WithMany()
                .HasForeignKey(property => property.LanguageId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}