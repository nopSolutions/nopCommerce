using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class BlogPostBuilder : BaseEntityBuilder<BlogPost>
    {
        #region Methods
        
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(BlogPost.Title)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(BlogPost.Body)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(BlogPost.MetaKeywords)).AsString(400).Nullable()
                .WithColumn(nameof(BlogPost.MetaTitle)).AsString(400).Nullable()
                .WithColumn(nameof(BlogPost.LanguageId))
                    .AsInt32()
                    .ForeignKey<Language>();
        }

        #endregion
    }
}