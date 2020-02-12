using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<ReturnRequest> builder)
        {
            builder.HasTableName(nameof(ReturnRequest));

            builder.Property(returnRequest => returnRequest.ReasonForReturn).IsNullable(false);
            builder.Property(returnRequest => returnRequest.RequestedAction).IsNullable(false);
            builder.Property(returnrequest => returnrequest.CustomNumber);
            builder.Property(returnrequest => returnrequest.StoreId);
            builder.Property(returnrequest => returnrequest.OrderItemId);
            builder.Property(returnrequest => returnrequest.CustomerId);
            builder.Property(returnrequest => returnrequest.Quantity);
            builder.Property(returnrequest => returnrequest.CustomerComments);
            builder.Property(returnrequest => returnrequest.UploadedFileId);
            builder.Property(returnrequest => returnrequest.StaffNotes);
            builder.Property(returnrequest => returnrequest.ReturnRequestStatusId);
            builder.Property(returnrequest => returnrequest.CreatedOnUtc);
            builder.Property(returnrequest => returnrequest.UpdatedOnUtc);

            builder.Ignore(returnRequest => returnRequest.ReturnRequestStatus);
        }

        #endregion
    }
}