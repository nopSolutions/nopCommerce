using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<MessageTemplate> builder)
        {
            builder.HasTableName(nameof(MessageTemplate));

            builder.Property(template => template.Name).HasLength(200);
            builder.HasColumn(template => template.Name).IsColumnRequired();
            builder.Property(template => template.BccEmailAddresses).HasLength(200);
            builder.Property(template => template.Subject).HasLength(1000);
            builder.HasColumn(template => template.EmailAccountId).IsColumnRequired();
          
            builder.Property(template => template.Body);
            builder.Property(template => template.IsActive);
            builder.Property(template => template.DelayBeforeSend);
            builder.Property(template => template.DelayPeriodId);
            builder.Property(template => template.AttachedDownloadId);
            builder.Property(template => template.LimitedToStores);

            builder.Ignore(template => template.DelayPeriod);
        }

        #endregion
    }
}