using LinqToDB.Mapping;
using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
    /// <summary>
    /// Represents a news comment mapping configuration
    /// </summary>
    public partial class NewsCommentMap : NopEntityTypeConfiguration<NewsComment>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<NewsComment> builder)
        {
            builder.HasTableName(nameof(NewsComment));
            builder.HasPrimaryKey(comment => comment.Id);

            builder.Property(comment => comment.CommentTitle);
            builder.Property(comment => comment.CommentText);
            builder.Property(comment => comment.NewsItemId);
            builder.Property(comment => comment.CustomerId);
            builder.Property(comment => comment.IsApproved);
            builder.Property(comment => comment.StoreId);
            builder.Property(comment => comment.CreatedOnUtc);
        }

        #endregion
    }
}