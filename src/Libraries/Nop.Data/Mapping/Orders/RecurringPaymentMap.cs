using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    public partial class RecurringPaymentMap : NopEntityTypeConfiguration<RecurringPayment>
    {
        public override void Configure(EntityTypeBuilder<RecurringPayment> builder)
        {
            base.Configure(builder);
            builder.ToTable("RecurringPayment");
            builder.HasKey(rp => rp.Id);

            builder.Ignore(rp => rp.NextPaymentDate);
            builder.Ignore(rp => rp.CyclesRemaining);
            builder.Ignore(rp => rp.CyclePeriod);



            //builder.HasRequired(rp => rp.InitialOrder).WithOptional().Map(x => x.MapKey("InitialOrderId")).WillCascadeOnDelete(false);
            builder.HasOne(rp => rp.InitialOrder)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(o => o.InitialOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}