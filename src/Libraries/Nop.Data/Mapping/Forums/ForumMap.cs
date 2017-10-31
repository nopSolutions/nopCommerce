using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ForumMap : NopEntityTypeConfiguration<Forum>
    {
        public override void Configure(EntityTypeBuilder<Forum> builder)
        {
            base.Configure(builder);
            builder.ToTable("Forums_Forum");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Name).IsRequired().HasMaxLength(200);

            builder.HasOne(f => f.ForumGroup)
                .WithMany(fg => fg.Forums)
                .IsRequired(true)
                .HasForeignKey(f => f.ForumGroupId);
        }
    }
}
