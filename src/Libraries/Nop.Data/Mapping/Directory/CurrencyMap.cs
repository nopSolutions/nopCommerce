using LinqToDB.Mapping;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Represents a currency mapping configuration
    /// </summary>
    public partial class CurrencyMap : NopEntityTypeConfiguration<Currency>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Currency> builder)
        {
            builder.HasTableName(nameof(Currency));

            builder.Property(currency => currency.Name).HasLength(50).IsNullable(false);
            builder.Property(currency => currency.CurrencyCode).HasLength(5).IsNullable(false);
            builder.Property(currency => currency.DisplayLocale).HasLength(50);
            builder.Property(currency => currency.CustomFormatting).HasLength(50);
            builder.Property(currency => currency.Rate).HasDecimal();
            builder.Property(currency => currency.LimitedToStores);
            builder.Property(currency => currency.Published);
            builder.Property(currency => currency.DisplayOrder);
            builder.Property(currency => currency.CreatedOnUtc);
            builder.Property(currency => currency.UpdatedOnUtc);
            builder.Property(currency => currency.RoundingTypeId);

            builder.Ignore(currency => currency.RoundingType);
        }

        #endregion
    }
}