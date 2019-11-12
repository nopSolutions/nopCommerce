using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<BackInStockSubscription> builder)
        {
            builder.HasTableName(nameof(BackInStockSubscription));

            builder.Property(backinstocksubscription => backinstocksubscription.StoreId);
            builder.Property(backinstocksubscription => backinstocksubscription.ProductId);
            builder.Property(backinstocksubscription => backinstocksubscription.CustomerId);
            builder.Property(backinstocksubscription => backinstocksubscription.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(subscription => subscription.Product)
            //    .WithMany()
            //    .HasForeignKey(subscription => subscription.ProductId)
            //    .IsColumnRequired();

            //builder.HasOne(subscription => subscription.Customer)
            //    .WithMany()
            //    .HasForeignKey(subscription => subscription.CustomerId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}