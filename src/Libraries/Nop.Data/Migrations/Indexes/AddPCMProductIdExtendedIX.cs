using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 12:02:35:9280389")]
    public class AddPCMProductIdExtendedIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_PCM_ProductId_Extended").OnTable(NameCompatibilityManager.GetTableName(typeof(ProductCategory)))
                .OnColumn(nameof(ProductCategory.ProductId)).Ascending()
                .OnColumn(nameof(ProductCategory.IsFeaturedProduct)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(ProductCategory.CategoryId));
        }

        #endregion
    }
}