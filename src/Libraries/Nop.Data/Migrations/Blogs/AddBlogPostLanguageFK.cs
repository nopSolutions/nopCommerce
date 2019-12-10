using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Migrations.Blogs
{
    [Migration(637097607595956342)]
    public class AddBlogPostLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(BlogPost))
                .ForeignColumn(nameof(BlogPost.LanguageId))
                .ToTable(nameof(Language))
                .PrimaryColumn(nameof(Language.Id));

            Create.Index().OnTable(nameof(BlogPost)).OnColumn(nameof(BlogPost.LanguageId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}