using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class MeasureDimensionMap : NopEntityTypeConfiguration<MeasureDimension>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public MeasureDimensionMap()
        {
            this.ToTable("MeasureDimension");
            this.HasKey(m => m.Id);
            this.Property(m => m.Name).IsRequired().HasMaxLength(100);
            this.Property(m => m.SystemKeyword).IsRequired().HasMaxLength(100);
            this.Property(m => m.Ratio).HasPrecision(18, 8);
        }
    }
}