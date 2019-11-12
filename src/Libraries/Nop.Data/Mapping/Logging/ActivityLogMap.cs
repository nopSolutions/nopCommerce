using LinqToDB.Mapping;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Mapping.Logging
{
    /// <summary>
    /// Represents an activity log mapping configuration
    /// </summary>
    public partial class ActivityLogMap : NopEntityTypeConfiguration<ActivityLog>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ActivityLog> builder)
        {
            builder.HasTableName(nameof(ActivityLog));

            builder.HasColumn(logItem => logItem.Comment).IsColumnRequired();
            builder.Property(logItem => logItem.IpAddress).HasLength(200);
            builder.Property(logItem => logItem.EntityName).HasLength(400);

            builder.Property(logItem => logItem.ActivityLogTypeId);
            builder.Property(logItem => logItem.EntityId);
            builder.Property(logItem => logItem.CustomerId);
            builder.Property(logItem => logItem.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(logItem => logItem.ActivityLogType)
            //    .WithMany()
            //    .HasForeignKey(logItem => logItem.ActivityLogTypeId)
            //    .IsColumnRequired();

            //builder.HasOne(logItem => logItem.Customer)
            //    .WithMany()
            //    .HasForeignKey(logItem => logItem.CustomerId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}