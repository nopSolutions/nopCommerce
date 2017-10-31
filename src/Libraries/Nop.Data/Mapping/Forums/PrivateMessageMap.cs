using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class PrivateMessageMap : NopEntityTypeConfiguration<PrivateMessage>
    {
        public override void Configure(EntityTypeBuilder<PrivateMessage> builder)
        {
            base.Configure(builder);
            builder.ToTable("Forums_PrivateMessage");
            builder.HasKey(pm => pm.Id);
            builder.Property(pm => pm.Subject).IsRequired().HasMaxLength(450);
            builder.Property(pm => pm.Text).IsRequired();

            builder.HasOne(pm => pm.FromCustomer)
               .WithMany()
               .IsRequired(true)
               .HasForeignKey(pm => pm.FromCustomerId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pm => pm.ToCustomer)
               .WithMany()
               .IsRequired(true)
               .HasForeignKey(pm => pm.ToCustomerId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
