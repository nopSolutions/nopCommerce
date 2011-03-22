
using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;


namespace Nop.Data.Mapping.Customers
{
    public partial class RewardPointsHistoryMap : EntityTypeConfiguration<RewardPointsHistory>
    {
        public RewardPointsHistoryMap()
        {
            this.ToTable("RewardPointsHistory");
            this.HasKey(rph => rph.Id);

            this.Property(rph => rph.UsedAmount).HasPrecision(18, 4);
            //this.Property(rph => rph.UsedAmountInCustomerCurrency).HasPrecision(18, 4);

            this.HasRequired(rph => rph.Customer)
                .WithMany(c => c.RewardPointsHistory)
                .HasForeignKey(rph => rph.CustomerId);

            this.HasOptional(rph => rph.UsedWithOrder)
                .WithOptionalDependent(o => o.RedeemedRewardPointsEntry)
                .WillCascadeOnDelete(false);
        }
    }
}