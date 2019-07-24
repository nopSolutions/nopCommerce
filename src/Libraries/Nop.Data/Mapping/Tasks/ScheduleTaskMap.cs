using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<ScheduleTask> builder)
        {
            builder.ToTable(nameof(ScheduleTask));
            builder.HasKey(task => task.Id);

            builder.Property(task => task.Name).IsRequired();
            builder.Property(task => task.Type).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}