using System.Data;
using FluentMigrator;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.News
{
    [Migration(637097798362530774)]
    public class AddNewsCommentStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(NewsComment)
                , nameof(NewsComment.StoreId)
                , nameof(Store)
                , nameof(Store.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}