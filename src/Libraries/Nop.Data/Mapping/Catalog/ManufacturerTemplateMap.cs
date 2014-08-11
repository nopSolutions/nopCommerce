using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ManufacturerTemplateMap : NopEntityTypeConfiguration<ManufacturerTemplate>
    {
        public ManufacturerTemplateMap()
        {
            this.ToTable("ManufacturerTemplate");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}