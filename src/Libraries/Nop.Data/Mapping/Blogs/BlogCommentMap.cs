using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Blogs;

namespace Nop.Data.Mapping.Blogs
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class BlogCommentMap : NopEntityTypeConfiguration<BlogComment>
    {
        public override void Configure(EntityTypeBuilder<BlogComment> builder)
        {
            base.Configure(builder);
            builder.ToTable("BlogComment");
            builder.HasKey(comment => comment.Id);
            builder.HasOne(comment => comment.BlogPost)
                .WithMany(blog => blog.BlogComments)
                .HasForeignKey(comment => comment.BlogPostId);

            builder.HasOne(comment => comment.Customer)
                .WithMany()
                .HasForeignKey(comment => comment.CustomerId);

            builder.HasOne(comment => comment.Store)
                .WithMany()
                .HasForeignKey(comment => comment.StoreId);
        }
    }
}