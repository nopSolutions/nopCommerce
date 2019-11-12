using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a tier price mapping configuration
    /// </summary>
    public partial class TierPriceMap : NopEntityTypeConfiguration<TierPrice>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<TierPrice> builder)
        {
            builder.HasTableName(nameof(TierPrice));

            builder.Property(price => price.Price).HasDbType("decimal(18, 4)");
            builder.Property(price => price.ProductId);
            builder.Property(price => price.StoreId);
            builder.Property(price => price.CustomerRoleId);
            builder.Property(price => price.Quantity);
            builder.Property(price => price.StartDateTimeUtc);
            builder.Property(price => price.EndDateTimeUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(price => price.Product)
            //    .WithMany(product => product.TierPrices)
            //    .HasForeignKey(price => price.ProductId)
            //    .IsColumnRequired();

            //builder.HasOne(price => price.CustomerRole)
            //    .WithMany()
            //    .HasForeignKey(price => price.CustomerRoleId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion
    }
}