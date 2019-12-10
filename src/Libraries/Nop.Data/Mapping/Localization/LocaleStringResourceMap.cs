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

            builder.Property(locale => locale.ResourceName).HasLength(200).IsNullable(false);
            builder.Property(locale => locale.ResourceValue).IsNullable(false);

            builder.Property(localestringresource => localestringresource.LanguageId);
        }

        #endregion
    }
}