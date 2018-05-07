using Nop.Core.Domain.Tax;

namespace Nop.Data.Mapping.Tax
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class TaxCategoryMap : NopEntityTypeConfiguration<TaxCategory>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public TaxCategoryMap()
        {
            this.ToTable("TaxCategory");
            this.HasKey(tc => tc.Id);
            this.Property(tc => tc.Name).IsRequired().HasMaxLength(400);
        }
    }
}
