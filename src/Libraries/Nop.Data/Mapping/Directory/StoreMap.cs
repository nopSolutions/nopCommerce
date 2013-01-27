using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    public partial class StoreMap : EntityTypeConfiguration<Store>
    {
        public StoreMap()
        {
            this.ToTable("Store");
            this.HasKey(s => s.Id);
            this.Property(s => s.Name).IsRequired().HasMaxLength(400);
        }
    }
}