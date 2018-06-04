using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Security;

namespace Nop.Data.Mapping.Security
{
    /// <summary>
    /// Represents an ACL record mapping configuration
    /// </summary>
    public partial class AclRecordMap : NopEntityTypeConfiguration<AclRecord>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<AclRecord> builder)
        {
            builder.ToTable(nameof(AclRecord));
            builder.HasKey(record => record.Id);

            builder.Property(record => record.EntityName).HasMaxLength(400).IsRequired();

            builder.HasOne(record => record.CustomerRole)
                .WithMany()
                .HasForeignKey(record => record.CustomerRoleId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}