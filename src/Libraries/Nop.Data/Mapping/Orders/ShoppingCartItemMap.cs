using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a shopping cart item mapping configuration
    /// </summary>
    public partial class ShoppingCartItemMap : NopEntityTypeConfiguration<ShoppingCartItem>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.ToTable(nameof(ShoppingCartItem));
            builder.HasKey(item => item.Id);

            builder.Property(item => item.CustomerEnteredPrice).HasColumnType("decimal(18, 4)");

            builder.HasOne(item => item.Customer)
                .WithMany(customer => customer.ShoppingCartItems)
                .HasForeignKey(item => item.CustomerId)
                .IsRequired();

            builder.HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .IsRequired();

            builder.Ignore(item => item.ShoppingCartType);

            base.Configure(builder);
        }

        #endregion
    }
}