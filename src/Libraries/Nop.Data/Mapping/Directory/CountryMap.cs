using LinqToDB.Mapping;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Represents a country mapping configuration
    /// </summary>
    public partial class CountryMap : NopEntityTypeConfiguration<Country>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Country> builder)
        {
            builder.HasTableName(nameof(Country));

            builder.Property(country => country.Name).HasLength(100).IsNullable(false);
            builder.Property(country => country.TwoLetterIsoCode).HasLength(2);
            builder.Property(country => country.ThreeLetterIsoCode).HasLength(3);
            builder.Property(country => country.AllowsBilling);
            builder.Property(country => country.AllowsShipping);
            builder.Property(country => country.NumericIsoCode);
            builder.Property(country => country.SubjectToVat);
            builder.Property(country => country.Published);
            builder.Property(country => country.DisplayOrder);
            builder.Property(country => country.LimitedToStores);
        }

        #endregion
    }
}