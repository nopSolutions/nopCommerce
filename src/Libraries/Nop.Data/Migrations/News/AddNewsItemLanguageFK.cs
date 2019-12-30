using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.News
{
    [Migration(637097800094361423)]
    public class AddNewsItemLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.NewsItemTable,
                nameof(NewsItem.LanguageId),
                nameof(Language),
                nameof(Language.Id),
                Rule.Cascade);
        }

        #endregion
    }
}