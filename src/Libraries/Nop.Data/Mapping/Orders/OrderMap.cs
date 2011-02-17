

using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;


namespace Nop.Data.Mapping.Orders
{
    public partial class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            this.ToTable("Order");
            this.HasKey(o => o.Id);
            
            this.HasRequired(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);
        }
    }
}