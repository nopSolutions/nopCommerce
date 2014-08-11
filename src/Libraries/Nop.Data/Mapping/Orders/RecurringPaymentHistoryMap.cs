using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    public partial class RecurringPaymentHistoryMap : NopEntityTypeConfiguration<RecurringPaymentHistory>
    {
        public RecurringPaymentHistoryMap()
        {
            this.ToTable("RecurringPaymentHistory");
            this.HasKey(rph => rph.Id);

            this.HasRequired(rph => rph.RecurringPayment)
                .WithMany(rp => rp.RecurringPaymentHistory)
                .HasForeignKey(rph => rph.RecurringPaymentId);

            //entity framework issue if we have navigation property to 'Order'
            //1. save recurring payment with an order
            //2. save recurring payment history with an order
            //3. update associated order => exception is thrown
        }
    }
}