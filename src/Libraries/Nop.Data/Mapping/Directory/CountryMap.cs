using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CountryMap : NopEntityTypeConfiguration<Country>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CountryMap()
        {
            this.ToTable("Country");
            this.HasKey(c =>c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(100);
            this.Property(c =>c.TwoLetterIsoCode).HasMaxLength(2);
            this.Property(c =>c.ThreeLetterIsoCode).HasMaxLength(3);
        }
    }
}