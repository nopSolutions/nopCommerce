using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Mapping.Stores
{
    /// <summary>
    /// Represents a store mapping configuration
    /// </summary>
    public partial class StoreMap : NopEntityTypeConfiguration<Store>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable(nameof(Store));
            builder.HasKey(store => store.Id);

            builder.Property(store => store.Name).HasMaxLength(400).IsRequired();
            builder.Property(store => store.Url).HasMaxLength(400).IsRequired();
            builder.Property(store => store.Hosts).HasMaxLength(1000);
            builder.Property(store => store.CompanyName).HasMaxLength(1000);
            builder.Property(store => store.CompanyAddress).HasMaxLength(1000);
            builder.Property(store => store.CompanyPhoneNumber).HasMaxLength(1000);
            builder.Property(store => store.CompanyVat).HasMaxLength(1000);

            base.Configure(builder);
        }

        #endregion
    }
}