using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Mapping.Logging
{
    public partial class ActivityLogTypeMap : NopEntityTypeConfiguration<ActivityLogType>
    {
        public override void Configure(EntityTypeBuilder<ActivityLogType> builder)
        {
            base.Configure(builder);
            builder.ToTable("ActivityLogType");
            builder.HasKey(alt => alt.Id);

            builder.Property(alt => alt.SystemKeyword).IsRequired().HasMaxLength(100);
            builder.Property(alt => alt.Name).IsRequired().HasMaxLength(200);
        }
    }
}
