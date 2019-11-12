using LinqToDB.Mapping;
using Nop.Core.Domain.Blogs;

namespace Nop.Data.Mapping.Blogs
{
    /// <summary>
    /// Represents a blog comment mapping configuration
    /// </summary>
    public partial class BlogCommentMap : NopEntityTypeConfiguration<BlogComment>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<BlogComment> builder)
        {
            builder.HasTableName(nameof(BlogComment));

            builder.Property(blogcomment => blogcomment.CustomerId);
            builder.Property(blogcomment => blogcomment.CommentText);
            builder.Property(blogcomment => blogcomment.IsApproved);
            builder.Property(blogcomment => blogcomment.StoreId);
            builder.Property(blogcomment => blogcomment.BlogPostId);
            builder.Property(blogcomment => blogcomment.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(comment => comment.BlogPost)
            //    .WithMany(blog => blog.BlogComments)
            //    .HasForeignKey(comment => comment.BlogPostId)
            //    .IsColumnRequired();

            //builder.HasOne(comment => comment.Customer)
            //    .WithMany()
            //    .HasForeignKey(comment => comment.CustomerId)
            //    .IsColumnRequired();

            //builder.HasOne(comment => comment.Store)
            //    .WithMany()
            //    .HasForeignKey(comment => comment.StoreId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}