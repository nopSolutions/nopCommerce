using LinqToDB.Mapping;
using Nop.Core.Domain.Tasks;

namespace Nop.Data.Mapping.Tasks
{
    /// <summary>
    /// Represents a schedule task mapping configuration
    /// </summary>
    public partial class ScheduleTaskMap : NopEntityTypeConfiguration<ScheduleTask>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ScheduleTask> builder)
        {
            builder.HasTableName(nameof(ScheduleTask));

            builder.HasColumn(task => task.Name).IsColumnRequired();
            builder.HasColumn(task => task.Type).IsColumnRequired();

            builder.Property(task => task.Seconds);
            builder.Property(task => task.Enabled);
            builder.Property(task => task.StopOnError);
            builder.Property(task => task.LastStartUtc);
            builder.Property(task => task.LastEndUtc);
            builder.Property(task => task.LastSuccessUtc);
        }

        #endregion
    }
}