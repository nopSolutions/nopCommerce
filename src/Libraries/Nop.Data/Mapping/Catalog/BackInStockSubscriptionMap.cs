using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a back in stock subscription mapping configuration
    /// </summary>
    public partial class BackInStockSubscriptionMap : NopEntityTypeConfiguration<BackInStockSubscription>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<BackInStockSubscription> builder)
        {
            builder.ToTable(nameof(BackInStockSubscription));
            builder.HasKey(subscription => subscription.Id);

            builder.HasOne(subscription => subscription.Product)
                .WithMany()
                .HasForeignKey(subscription => subscription.ProductId)
                .IsRequired();

            builder.HasOne(subscription => subscription.Customer)
                .WithMany()
                .HasForeignKey(subscription => subscription.CustomerId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}