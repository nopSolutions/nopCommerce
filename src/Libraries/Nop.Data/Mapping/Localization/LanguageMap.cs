using LinqToDB.Mapping;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Mapping.Localization
{
    /// <summary>
    /// Represents a language mapping configuration
    /// </summary>
    public partial class LanguageMap : NopEntityTypeConfiguration<Language>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Language> builder)
        {
            builder.HasTableName(nameof(Language));

            builder.Property(language => language.Name).HasLength(100).IsNullable(false);
            builder.Property(language => language.LanguageCulture).HasLength(20).IsNullable(false);
            builder.Property(language => language.UniqueSeoCode).HasLength(2);
            builder.Property(language => language.FlagImageFileName).HasLength(50);
            builder.Property(language => language.Rtl);
            builder.Property(language => language.LimitedToStores);
            builder.Property(language => language.DefaultCurrencyId);
            builder.Property(language => language.Published);
            builder.Property(language => language.DisplayOrder);
        }

        #endregion
    }
}