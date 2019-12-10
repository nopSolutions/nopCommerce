using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;

namespace Nop.Data.Migrations.News
{
    [Migration(637097800094361423)]
    public class AddNewsItemLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.NewsItemTable)
                .ForeignColumn(nameof(NewsItem.LanguageId))
                .ToTable(nameof(Language))
                .PrimaryColumn(nameof(Language.Id));

            Create.Index().OnTable(NopMappingDefaults.NewsItemTable).OnColumn(nameof(NewsItem.LanguageId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}