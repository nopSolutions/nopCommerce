using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<QueuedEmail> builder)
        {
            builder.ToTable(nameof(QueuedEmail));
            builder.HasKey(email => email.Id);

            builder.Property(email => email.From).HasMaxLength(500).IsRequired();
            builder.Property(email => email.FromName).HasMaxLength(500);
            builder.Property(email => email.To).HasMaxLength(500).IsRequired();
            builder.Property(email => email.ToName).HasMaxLength(500);
            builder.Property(email => email.ReplyTo).HasMaxLength(500);
            builder.Property(email => email.ReplyToName).HasMaxLength(500);
            builder.Property(email => email.CC).HasMaxLength(500);
            builder.Property(email => email.Bcc).HasMaxLength(500);
            builder.Property(email => email.Subject).HasMaxLength(1000);

            builder.HasOne(email => email.EmailAccount)
                .WithMany()
                .HasForeignKey(email => email.EmailAccountId)
                .IsRequired();

            builder.Ignore(email => email.Priority);

            base.Configure(builder);
        }

        #endregion
    }
}