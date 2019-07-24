using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Mapping.Stores
{
    /// <summary>
    /// Represents a store mapping mapping configuration
    /// </summary>
    public partial class StoreMappingMap : NopEntityTypeConfiguration<StoreMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<StoreMapping> builder)
        {
            builder.ToTable(nameof(StoreMapping));
            builder.HasKey(storeMapping => storeMapping.Id);

            builder.Property(storeMapping => storeMapping.EntityName).HasMaxLength(400).IsRequired();

            builder.HasOne(storeMapping => storeMapping.Store)
                .WithMany()
                .HasForeignKey(storeMapping => storeMapping.StoreId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}