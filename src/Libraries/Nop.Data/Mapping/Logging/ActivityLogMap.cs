using Nop.Core.Domain.Logging;

namespace Nop.Data.Mapping.Logging
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ActivityLogMap : NopEntityTypeConfiguration<ActivityLog>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ActivityLogMap()
        {
            this.ToTable("ActivityLog");
            this.HasKey(logItem => logItem.Id);
            this.Property(logItem => logItem.Comment).IsRequired();
            this.Property(logItem => logItem.IpAddress).HasMaxLength(200);
            this.Property(logItem => logItem.EntityName).HasMaxLength(400);

            this.HasRequired(logItem => logItem.ActivityLogType)
                .WithMany()
                .HasForeignKey(logItem => logItem.ActivityLogTypeId);

            this.HasRequired(logItem => logItem.Customer)
                .WithMany()
                .HasForeignKey(logItem => logItem.CustomerId);
        }
    }
}