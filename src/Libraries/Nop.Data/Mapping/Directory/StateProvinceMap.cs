using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<StateProvince> builder)
        {
            builder.ToTable(nameof(StateProvince));
            builder.HasKey(state => state.Id);

            builder.Property(state => state.Name).HasMaxLength(100).IsRequired();
            builder.Property(state => state.Abbreviation).HasMaxLength(100);

            builder.HasOne(state => state.Country)
                .WithMany(country => country.StateProvinces)
                .HasForeignKey(state => state.CountryId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}