using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Represents a message template mapping configuration
    /// </summary>
    public partial class MessageTemplateMap : NopEntityTypeConfiguration<MessageTemplate>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<MessageTemplate> builder)
        {
            builder.ToTable(nameof(MessageTemplate));
            builder.HasKey(template => template.Id);

            builder.Property(template => template.Name).HasMaxLength(200).IsRequired();
            builder.Property(template => template.BccEmailAddresses).HasMaxLength(200);
            builder.Property(template => template.Subject).HasMaxLength(1000);
            builder.Property(template => template.EmailAccountId).IsRequired();

            builder.Ignore(template => template.DelayPeriod);

            base.Configure(builder);
        }

        #endregion
    }
}