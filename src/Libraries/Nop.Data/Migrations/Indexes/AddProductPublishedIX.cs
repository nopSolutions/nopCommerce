using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037703")]
    public class AddProductPublishedIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_Product_Published").OnTable(nameof(Product))
                .OnColumn(nameof(Product.Published)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}