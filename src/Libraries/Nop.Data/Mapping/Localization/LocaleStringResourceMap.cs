using LinqToDB.Mapping;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Mapping.Localization
{
    /// <summary>
    /// Represents a locale string resource mapping configuration
    /// </summary>
    public partial class LocaleStringResourceMap : NopEntityTypeConfiguration<LocaleStringResource>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<LocaleStringResource> builder)
        {
            builder.HasTableName(nameof(LocaleStringResource));

            builder.Property(locale => locale.ResourceName).HasLength(200);
            builder.HasColumn(locale => locale.ResourceName).IsColumnRequired();
            builder.HasColumn(locale => locale.ResourceValue).IsColumnRequired();

            builder.Property(localestringresource => localestringresource.LanguageId);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(locale => locale.Language)
            //    .WithMany()
            //    .HasForeignKey(locale => locale.LanguageId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}