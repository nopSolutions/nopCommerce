

using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Orders;


namespace Nop.Data.Mapping.Orders
{
    public partial class RecurringPaymentHistoryMap : EntityTypeConfiguration<RecurringPaymentHistory>
    {
        public RecurringPaymentHistoryMap()
        {
            this.ToTable("RecurringPaymentHistory");
            this.HasKey(rph => rph.Id);

            this.HasRequired(rph => rph.RecurringPayment)
                .WithMany(rp => rp.RecurringPaymentHistory)
                .HasForeignKey(rph => rph.RecurringPaymentId);

            this.HasRequired(rph => rph.Order).WithOptional();
        }
    }
}