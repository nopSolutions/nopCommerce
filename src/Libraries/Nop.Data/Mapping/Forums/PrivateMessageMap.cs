using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;
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
        public override void Configure(EntityTypeBuilder<PrivateMessage> builder)
        {
            builder.ToTable(NopMappingDefaults.PrivateMessageTable);
            builder.HasKey(message => message.Id);

            builder.Property(message => message.Subject).HasMaxLength(450).IsRequired();
            builder.Property(message => message.Text).IsRequired();

            builder.HasOne<Customer>().WithMany().HasForeignKey(message => message.FromCustomerId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Customer>().WithMany().HasForeignKey(message => message.ToCustomerId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }

        #endregion
    }
}