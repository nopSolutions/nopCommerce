using LinqToDB.Mapping;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents an order note mapping configuration
    /// </summary>
    public partial class OrderNoteMap : NopEntityTypeConfiguration<OrderNote>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<OrderNote> builder)
        {
            builder.HasTableName(nameof(OrderNote));

            builder.Property(note => note.Note).IsNullable(false);

            builder.Property(note => note.OrderId);
            builder.Property(note => note.DownloadId);
            builder.Property(note => note.DisplayToCustomer);
            builder.Property(note => note.CreatedOnUtc);
        }

        #endregion
    }
}