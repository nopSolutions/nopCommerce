using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class StateProvinceMap : NopEntityTypeConfiguration<StateProvince>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public StateProvinceMap()
        {
            this.ToTable("StateProvince");
            this.HasKey(sp => sp.Id);
            this.Property(sp => sp.Name).IsRequired().HasMaxLength(100);
            this.Property(sp => sp.Abbreviation).HasMaxLength(100);

            this.HasRequired(sp => sp.Country)
                .WithMany(c => c.StateProvinces)
                .HasForeignKey(sp => sp.CountryId);
        }
    }
}