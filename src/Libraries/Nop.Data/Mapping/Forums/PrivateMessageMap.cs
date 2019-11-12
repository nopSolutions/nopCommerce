using LinqToDB.Mapping;
using Nop.Core.Domain.Forums;
using Nop.Data.Data;

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

            builder.Property(message => message.Subject).HasLength(450);
            builder.HasColumn(message => message.Subject).IsColumnRequired();
            builder.HasColumn(message => message.Text).IsColumnRequired();

            builder.Property(message => message.StoreId);
            builder.Property(message => message.FromCustomerId);
            builder.Property(message => message.ToCustomerId);
            builder.Property(message => message.IsRead);
            builder.Property(message => message.IsDeletedByAuthor);
            builder.Property(message => message.IsDeletedByRecipient);
            builder.Property(message => message.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(message => message.FromCustomer)
            //   .WithMany()
            //   .HasForeignKey(message => message.FromCustomerId)
            //   .IsColumnRequired()
            //   .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(message => message.ToCustomer)
            //   .WithMany()
            //   .HasForeignKey(message => message.ToCustomerId)
            //   .IsColumnRequired()
            //   .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion
    }
}