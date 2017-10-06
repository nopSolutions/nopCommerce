using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Blogs;

namespace Nop.Data.Mapping.Blogs
{
    public partial class BlogPostMap : NopEntityTypeConfiguration<BlogPost>
    {
        public override void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            base.Configure(builder);
            builder.ToTable("BlogPost");
            builder.HasKey(bp => bp.Id);
            builder.Property(bp => bp.Title).IsRequired();
            builder.Property(bp => bp.Body).IsRequired();
            builder.Property(bp => bp.MetaKeywords).HasMaxLength(400);
            builder.Property(bp => bp.MetaTitle).HasMaxLength(400);
            builder.HasOne(bp => bp.Language)
                .WithMany()
                .HasForeignKey(bp => bp.LanguageId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
        }
    }
}