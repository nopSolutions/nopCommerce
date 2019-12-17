using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Integration.Domains;

namespace Nop.Plugin.Integration.Data
{
    public partial class CustomTableMap : NopEntityTypeConfiguration<CustomTable>
    {
        public override void Configure(EntityTypeBuilder<CustomTable> builder)
        {
            builder.ToTable(nameof(CustomTable));

            builder.HasKey(rcoh => rcoh.Id);
        }
    }
}
