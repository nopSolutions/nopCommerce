using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Security;

namespace Nop.Data.Mapping.Security
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class PermissionRecordMap : NopEntityTypeConfiguration<PermissionRecord>
    {
        public override void Configure(EntityTypeBuilder<PermissionRecord> builder)
        {
            base.Configure(builder);
            builder.ToTable("PermissionRecord");
            builder.HasKey(pr => pr.Id);
            builder.Property(pr => pr.Name).IsRequired();
            builder.Property(pr => pr.SystemName).IsRequired().HasMaxLength(255);
            builder.Property(pr => pr.Category).IsRequired().HasMaxLength(255);
        }
    }
}