using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    public partial class RecurringPaymenbuildertoryMap : NopEntityTypeConfiguration<RecurringPaymentHistory>
    {
        public override void Configure(EntityTypeBuilder<RecurringPaymentHistory> builder)
        {
            base.Configure(builder);
            builder.ToTable("RecurringPaymentHistory");
            builder.HasKey(rph => rph.Id);

            builder.HasOne(rph => rph.RecurringPayment)
                .WithMany(rp => rp.RecurringPaymentHistory)
                .HasForeignKey(rph => rph.RecurringPaymentId);

            //entity framework issue if we have navigation property to 'Order'
            //1. save recurring payment with an order
            //2. save recurring payment history with an order
            //3. update associated order => exception is thrown
        }
    }
}