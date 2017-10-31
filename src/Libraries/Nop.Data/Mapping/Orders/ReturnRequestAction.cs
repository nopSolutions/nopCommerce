using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ReturnRequestActionMap : NopEntityTypeConfiguration<ReturnRequestAction>
    {
        public override void Configure(EntityTypeBuilder<ReturnRequestAction> builder)
        {
            base.Configure(builder);
            builder.ToTable("ReturnRequestAction");
            builder.HasKey(rra => rra.Id);
            builder.Property(rra => rra.Name).IsRequired().HasMaxLength(400);
        }
    }
}