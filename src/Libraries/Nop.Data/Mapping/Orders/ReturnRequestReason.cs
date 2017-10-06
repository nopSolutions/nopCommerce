using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    public partial class ReturnRequestReasonMap : NopEntityTypeConfiguration<ReturnRequestReason>
    {
        public override void Configure(EntityTypeBuilder<ReturnRequestReason> builder)
        {
            base.Configure(builder);
            builder.ToTable("ReturnRequestReason");
            builder.HasKey(rrr => rrr.Id);
            builder.Property(rrr => rrr.Name).IsRequired().HasMaxLength(400);
        }
    }
}