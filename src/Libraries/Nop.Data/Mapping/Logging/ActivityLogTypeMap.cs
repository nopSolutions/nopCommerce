using LinqToDB.Mapping;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Mapping.Logging
{
    /// <summary>
    /// Represents an activity log type mapping configuration
    /// </summary>
    public partial class ActivityLogTypeMap : NopEntityTypeConfiguration<ActivityLogType>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ActivityLogType> builder)
        {
            builder.HasTableName(nameof(ActivityLogType));

            builder.Property(logType => logType.SystemKeyword).HasLength(100);
            builder.Property(logType => logType.Name).HasLength(200);
            builder.HasColumn(logType => logType.SystemKeyword).IsColumnRequired();
            builder.HasColumn(logType => logType.Name).IsColumnRequired();
            builder.Property(activitylogtype => activitylogtype.Enabled);
        }

        #endregion
    }
}