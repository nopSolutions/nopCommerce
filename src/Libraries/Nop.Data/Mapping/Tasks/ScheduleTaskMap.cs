using Nop.Core.Domain.Tasks;

namespace Nop.Data.Mapping.Tasks
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ScheduleTaskMap : NopEntityTypeConfiguration<ScheduleTask>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ScheduleTaskMap()
        {
            this.ToTable("ScheduleTask");
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).IsRequired();
            this.Property(t => t.Type).IsRequired();
        }
    }
}