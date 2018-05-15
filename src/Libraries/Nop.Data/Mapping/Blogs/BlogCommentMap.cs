using Nop.Core.Domain.Blogs;

namespace Nop.Data.Mapping.Blogs
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class BlogCommentMap : NopEntityTypeConfiguration<BlogComment>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public BlogCommentMap()
        {
            this.ToTable("BlogComment");
            this.HasKey(comment => comment.Id);

            this.HasRequired(comment => comment.BlogPost)
                .WithMany(blog => blog.BlogComments)
                .HasForeignKey(comment => comment.BlogPostId);

            this.HasRequired(comment => comment.Customer)
                .WithMany()
                .HasForeignKey(comment => comment.CustomerId);

            this.HasRequired(comment => comment.Store)
                .WithMany()
                .HasForeignKey(comment => comment.StoreId);
        }
    }
}