using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a return request mapping configuration
    /// </summary>
    public partial class ReturnRequestMap : NopEntityTypeConfiguration<ReturnRequest>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ReturnRequest> builder)
        {
            builder.ToTable(nameof(ReturnRequest));
            builder.HasKey(returnRequest => returnRequest.Id);

            builder.Property(returnRequest => returnRequest.ReasonForReturn).IsRequired();
            builder.Property(returnRequest => returnRequest.RequestedAction).IsRequired();

            builder.HasOne(returnRequest => returnRequest.Customer)
                .WithMany(customer => customer.ReturnRequests)
                .HasForeignKey(returnRequest => returnRequest.CustomerId)
                .IsRequired();

            builder.Ignore(returnRequest => returnRequest.ReturnRequestStatus);

            base.Configure(builder);
        }

        #endregion
    }
}