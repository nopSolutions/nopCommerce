using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace Nop.Plugin.Api.DataMappings
{
    public class WebHooksMap : NopEntityTypeConfiguration<Domain.WebHooks>
    {
        public override void Configure(EntityTypeBuilder<Domain.WebHooks> builder)
        {
            builder.ToTable("WebHooks", "WebHooks");
            builder.HasKey(wh => new { wh.User, wh.Id });

            builder.Property(wh => wh.ProtectedData).IsRequired();
            builder.Property(wh => wh.RowVer).IsRowVersion();
        }
    }
}
