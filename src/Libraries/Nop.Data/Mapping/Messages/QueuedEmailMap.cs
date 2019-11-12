using LinqToDB.Mapping;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Represents a queued email mapping configuration
    /// </summary>
    public partial class QueuedEmailMap : NopEntityTypeConfiguration<QueuedEmail>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<QueuedEmail> builder)
        {
            builder.HasTableName(nameof(QueuedEmail));

            builder.HasColumn(email => email.From).IsColumnRequired();
            builder.Property(email => email.From).HasLength(500);
            builder.Property(email => email.FromName).HasLength(500);
            builder.HasColumn(email => email.To).IsColumnRequired();
            builder.Property(email => email.To).HasLength(500);
            builder.Property(email => email.ToName).HasLength(500);
            builder.Property(email => email.ReplyTo).HasLength(500);
            builder.Property(email => email.ReplyToName).HasLength(500);
            builder.Property(email => email.CC).HasLength(500);
            builder.Property(email => email.Bcc).HasLength(500);
            builder.Property(email => email.Subject).HasLength(1000);

            builder.Property(queuedemail => queuedemail.PriorityId);
            builder.Property(queuedemail => queuedemail.Body);
            builder.Property(queuedemail => queuedemail.AttachmentFilePath);
            builder.Property(queuedemail => queuedemail.AttachmentFileName);
            builder.Property(queuedemail => queuedemail.AttachedDownloadId);
            builder.Property(queuedemail => queuedemail.CreatedOnUtc);
            builder.Property(queuedemail => queuedemail.DontSendBeforeDateUtc);
            builder.Property(queuedemail => queuedemail.SentTries);
            builder.Property(queuedemail => queuedemail.SentOnUtc);
            builder.Property(queuedemail => queuedemail.EmailAccountId);

            builder.Ignore(email => email.Priority);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(email => email.EmailAccount)
            //    .WithMany()
            //    .HasForeignKey(email => email.EmailAccountId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}