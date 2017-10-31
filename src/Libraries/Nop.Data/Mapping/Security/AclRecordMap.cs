using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Security;

namespace Nop.Data.Mapping.Security
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class AclRecordMap : NopEntityTypeConfiguration<AclRecord>
    {
        public override void Configure(EntityTypeBuilder<AclRecord> builder)
        {
            base.Configure(builder);
            builder.ToTable("AclRecord");
            builder.HasKey(ar => ar.Id);

            builder.Property(ar => ar.EntityName).IsRequired().HasMaxLength(400);

            builder.HasOne(ar => ar.CustomerRole)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(ar => ar.CustomerRoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}