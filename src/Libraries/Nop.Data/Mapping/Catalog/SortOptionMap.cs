using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class SortOptionMap : NopEntityTypeConfiguration<SortOption>
    {
        public SortOptionMap()
        {
            this.ToTable("SortOption");
            this.HasKey(option => option.Id);

            this.Ignore(p => p.SortOptionType);
        }
    }
}
