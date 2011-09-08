using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Blogs;

namespace Nop.Data.Mapping.Blogs
{
    public partial class BlogPostMap : EntityTypeConfiguration<BlogPost>
    {
        public BlogPostMap()
        {
            this.ToTable("BlogPost");
            this.HasKey(bp => bp.Id);
            this.Property(bp => bp.Title).IsRequired().IsMaxLength();
            this.Property(bp => bp.Body).IsRequired().IsMaxLength();
            this.Property(bp => bp.Tags).IsMaxLength();

            this.HasRequired(bp => bp.Language)
                .WithMany()
                .HasForeignKey(bp => bp.LanguageId).WillCascadeOnDelete(true);
        }
    }
}