using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    public partial class ForumGroupMap : NopEntityTypeConfiguration<ForumGroup>
    {
        public override void Configure(EntityTypeBuilder<ForumGroup> builder)
        {
            base.Configure(builder);
            builder.ToTable("Forums_Group");
            builder.HasKey(fg => fg.Id);
            builder.Property(fg => fg.Name).IsRequired().HasMaxLength(200);
        }
    }
}
