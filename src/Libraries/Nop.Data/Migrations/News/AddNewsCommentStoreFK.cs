using FluentMigrator;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Migrations.News
{
    [Migration(637097798362530774)]
    public class AddNewsCommentStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(NewsComment))
                .ForeignColumn(nameof(NewsComment.StoreId))
                .ToTable(nameof(Store))
                .PrimaryColumn(nameof(Store.Id));

            Create.Index().OnTable(nameof(NewsComment)).OnColumn(nameof(NewsComment.StoreId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}