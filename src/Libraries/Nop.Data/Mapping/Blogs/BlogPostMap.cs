using LinqToDB.Mapping;
using Nop.Core.Domain.Blogs;

namespace Nop.Data.Mapping.Blogs
{
    /// <summary>
    /// Represents a blog post mapping configuration
    /// </summary>
    public partial class BlogPostMap : NopEntityTypeConfiguration<BlogPost>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<BlogPost> builder)
        {
            builder.HasTableName(nameof(BlogPost));

            builder.Property(blogPost => blogPost.Title).IsNullable(false);
            builder.Property(blogPost => blogPost.Body).IsNullable(false);
            builder.Property(blogPost => blogPost.MetaKeywords).HasLength(400);
            builder.Property(blogPost => blogPost.MetaTitle).HasLength(400);

            builder.Property(blogpost => blogpost.LanguageId);
            builder.Property(blogpost => blogpost.IncludeInSitemap);
            builder.Property(blogpost => blogpost.BodyOverview);
            builder.Property(blogpost => blogpost.AllowComments);
            builder.Property(blogpost => blogpost.Tags);
            builder.Property(blogpost => blogpost.StartDateUtc);
            builder.Property(blogpost => blogpost.EndDateUtc);
            builder.Property(blogpost => blogpost.MetaDescription);
            builder.Property(blogpost => blogpost.LimitedToStores);
            builder.Property(blogpost => blogpost.CreatedOnUtc);
        }

        #endregion
    }
}