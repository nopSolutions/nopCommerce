
using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Messages;


namespace Nop.Data.Mapping.Catalog
{
    public partial class QueuedEmailMap : EntityTypeConfiguration<QueuedEmail>
    {
        public QueuedEmailMap()
        {
            this.ToTable("QueuedEmail");
            this.HasKey(qe => qe.Id);

            this.Property(qe => qe.From).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.FromName).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.To).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.ToName).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.CC).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.Bcc).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.Subject).IsRequired().HasMaxLength(500);
            this.Property(qe => qe.Body).IsRequired().IsMaxLength();


            this.HasRequired(qe => qe.EmailAccount)
                .WithMany(ea => ea.QueuedEmails)
                .HasForeignKey(qe => qe.EmailAccountId);
        }
    }
}