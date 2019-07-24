using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.ToTable(nameof(BlogPost));
            builder.HasKey(blogPost => blogPost.Id);

            builder.Property(blogPost => blogPost.Title).IsRequired();
            builder.Property(blogPost => blogPost.Body).IsRequired();
            builder.Property(blogPost => blogPost.MetaKeywords).HasMaxLength(400);
            builder.Property(blogPost => blogPost.MetaTitle).HasMaxLength(400);

            builder.HasOne(blogPost => blogPost.Language)
                .WithMany()
                .HasForeignKey(blogPost => blogPost.LanguageId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}