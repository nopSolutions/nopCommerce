using Nop.Core.Domain.News;

namespace Nop.Data.Mapping.News
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class NewsItemMap : NopEntityTypeConfiguration<NewsItem>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public NewsItemMap()
        {
            this.ToTable("News");
            this.HasKey(ni => ni.Id);
            this.Property(ni => ni.Title).IsRequired();
            this.Property(ni => ni.Short).IsRequired();
            this.Property(ni => ni.Full).IsRequired();
            this.Property(ni => ni.MetaKeywords).HasMaxLength(400);
            this.Property(ni => ni.MetaTitle).HasMaxLength(400);

            this.HasRequired(ni => ni.Language)
                .WithMany()
                .HasForeignKey(ni => ni.LanguageId).WillCascadeOnDelete(true);
        }
    }
}