

using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Orders;


namespace Nop.Data.Mapping.Orders
{
    public partial class RecurringPaymentMap : EntityTypeConfiguration<RecurringPayment>
    {
        public RecurringPaymentMap()
        {
            this.ToTable("RecurringPayment");
            this.HasKey(rp => rp.Id);

            this.Ignore(rp => rp.NextPaymentDate);
            this.Ignore(rp => rp.CyclesRemaining);
            this.Ignore(rp => rp.CyclePeriod);
            
            this.HasRequired(rp => rp.InitialOrder).WithOptional();
        }
    }
}