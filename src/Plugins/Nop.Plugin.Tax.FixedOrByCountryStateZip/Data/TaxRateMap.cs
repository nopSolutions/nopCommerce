using LinqToDB.Mapping;
using Nop.Data;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Data
{
    /// <summary>
    /// Represents a tax rate mapping configuration
    /// </summary>
    public partial class TaxRateMap : NopEntityTypeConfiguration<TaxRate>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        public override void Configure(EntityMappingBuilder<TaxRate> builder)
        {
            builder.HasTableName(nameof(TaxRate));
            builder.Property(rate => rate.StoreId);
            builder.Property(rate => rate.TaxCategoryId);
            builder.Property(rate => rate.CountryId);
            builder.Property(rate => rate.StateProvinceId);
            builder.Property(rate => rate.Zip);
            builder.Property(rate => rate.Percentage).HasDbType("decimal(18, 4)");
        }

        #endregion
    }
}