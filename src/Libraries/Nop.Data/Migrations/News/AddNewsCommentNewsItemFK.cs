using System.Data;
using FluentMigrator;
using Nop.Core.Domain.News;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.News
{
    [Migration(637097798362530772)]
    public class AddNewsCommentNewsItemFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(NewsComment)
                , nameof(NewsComment.NewsItemId)
                , NopMappingDefaults.NewsItemTable
                , nameof(NewsItem.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}