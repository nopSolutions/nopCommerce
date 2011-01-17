using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    public partial class ShoppingCartItemMap : EntityTypeConfiguration<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            this.ToTable("ShoppingCartItem");
            this.HasKey(sci => sci.Id);

            this.Ignore(sci => sci.ShoppingCartType);

            this.HasRequired(sci => sci.Customer)
                .WithMany(c => c.ShoppingCartItems)
                .HasForeignKey(sci => sci.CustomerId);

            this.HasRequired(sci => sci.ProductVariant)
                .WithMany()
                .HasForeignKey(sci => sci.ProductVariantId);
        }
    }
}
