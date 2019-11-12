using LinqToDB.Mapping;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Represents a state and province mapping configuration
    /// </summary>
    public partial class StateProvinceMap : NopEntityTypeConfiguration<StateProvince>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<StateProvince> builder)
        {
            builder.HasTableName(nameof(StateProvince));

            builder.Property(state => state.Name).HasLength(100);
            builder.HasColumn(state => state.Name).IsColumnRequired();
            builder.Property(state => state.Abbreviation).HasLength(100);
            builder.Property(stateprovince => stateprovince.CountryId);
            builder.Property(stateprovince => stateprovince.Published);
            builder.Property(stateprovince => stateprovince.DisplayOrder);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(state => state.Country)
            //    .WithMany(country => country.StateProvinces)
            //    .HasForeignKey(state => state.CountryId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}