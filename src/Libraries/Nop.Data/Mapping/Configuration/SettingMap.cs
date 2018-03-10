using Nop.Core.Domain.Configuration;

namespace Nop.Data.Mapping.Configuration
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class SettingMap : NopEntityTypeConfiguration<Setting>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SettingMap()
        {
            this.ToTable("Setting");
            this.HasKey(s => s.Id);
            this.Property(s => s.Name).IsRequired().HasMaxLength(200);
            this.Property(s => s.Value).IsRequired().HasMaxLength(2000);
        }
    }
}