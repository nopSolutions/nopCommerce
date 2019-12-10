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

            builder.Property(logItem => logItem.Comment).IsNullable(false);
            builder.Property(logItem => logItem.IpAddress).HasLength(200);
            builder.Property(logItem => logItem.EntityName).HasLength(400);
            builder.Property(logItem => logItem.ActivityLogTypeId);
            builder.Property(logItem => logItem.EntityId);
            builder.Property(logItem => logItem.CustomerId);
            builder.Property(logItem => logItem.CreatedOnUtc);
        }

        #endregion
    }
}