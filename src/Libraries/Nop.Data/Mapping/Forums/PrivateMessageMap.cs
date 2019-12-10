using LinqToDB.Mapping;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a private message mapping configuration
    /// </summary>
    public partial class PrivateMessageMap : NopEntityTypeConfiguration<PrivateMessage>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<PrivateMessage> builder)
        {
            builder.HasTableName(NopMappingDefaults.PrivateMessageTable);

            builder.Property(message => message.Subject).HasLength(450).IsNullable(false);
            builder.Property(message => message.Text).IsNullable(false);

            builder.Property(message => message.StoreId);
            builder.Property(message => message.FromCustomerId);
            builder.Property(message => message.ToCustomerId);
            builder.Property(message => message.IsRead);
            builder.Property(message => message.IsDeletedByAuthor);
            builder.Property(message => message.IsDeletedByRecipient);
            builder.Property(message => message.CreatedOnUtc);
        }

        #endregion
    }
}