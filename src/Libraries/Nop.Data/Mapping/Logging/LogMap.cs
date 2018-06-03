using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Mapping.Logging
{
    /// <summary>
    /// Represents a log mapping configuration
    /// </summary>
    public partial class LogMap : NopEntityTypeConfiguration<Log>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable(nameof(Log));
            builder.HasKey(logItem => logItem.Id);

            builder.Property(logItem => logItem.ShortMessage).IsRequired();
            builder.Property(logItem => logItem.IpAddress).HasMaxLength(200);

            builder.Ignore(logItem => logItem.LogLevel);

            builder.HasOne(logItem => logItem.Customer)
                .WithMany()
                .HasForeignKey(logItem => logItem.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }

        #endregion
    }
}