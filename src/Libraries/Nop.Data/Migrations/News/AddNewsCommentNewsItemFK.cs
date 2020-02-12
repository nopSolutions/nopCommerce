using System.Data;
using FluentMigrator;
using Nop.Core.Domain.News;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.News
{
    [NopMigration("2019/11/19 05:03:56:2530772")]
    public class AddNewsCommentNewsItemFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(NewsComment),
                nameof(NewsComment.NewsItemId),
                NopMappingDefaults.NewsItemTable,
                nameof(NewsItem.Id),
                Rule.Cascade);
        }

        #endregion
    }
}