using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Content.Forums
{
    public partial class PrivateMessageMap : EntityTypeConfiguration<PrivateMessage>
    {
        public PrivateMessageMap()
        {
            this.ToTable("Forums_PrivateMessage");
            this.HasKey(pm => pm.Id);
            this.Property(pm => pm.Subject).IsRequired().HasMaxLength(450);
            this.Property(pm => pm.Text).IsRequired().IsMaxLength();

            this.HasRequired(pm => pm.FromCustomer)
               .WithMany()
               .HasForeignKey(pm => pm.FromUserId)
               .WillCascadeOnDelete(false);

            this.HasRequired(pm => pm.ToCustomer)
               .WithMany()
               .HasForeignKey(pm => pm.ToUserId)
               .WillCascadeOnDelete(false);
        }
    }
}
