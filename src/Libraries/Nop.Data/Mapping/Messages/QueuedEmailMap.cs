using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class QueuedEmailMap : NopEntityTypeConfiguration<QueuedEmail>
    {
        public override void Configure(EntityTypeBuilder<QueuedEmail> builder)
        {
            base.Configure(builder);
            builder.ToTable("QueuedEmail");
            builder.HasKey(qe => qe.Id);

            builder.Property(qe => qe.From).IsRequired().HasMaxLength(500);
            builder.Property(qe => qe.FromName).HasMaxLength(500);
            builder.Property(qe => qe.To).IsRequired().HasMaxLength(500);
            builder.Property(qe => qe.ToName).HasMaxLength(500);
            builder.Property(qe => qe.ReplyTo).HasMaxLength(500);
            builder.Property(qe => qe.ReplyToName).HasMaxLength(500);
            builder.Property(qe => qe.CC).HasMaxLength(500);
            builder.Property(qe => qe.Bcc).HasMaxLength(500);
            builder.Property(qe => qe.Subject).HasMaxLength(1000);


            builder.Ignore(qe => qe.Priority);

            builder.HasOne(qe => qe.EmailAccount)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(qe => qe.EmailAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}