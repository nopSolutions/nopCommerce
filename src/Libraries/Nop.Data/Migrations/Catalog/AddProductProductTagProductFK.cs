using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097631880193450)]
    public class AddAddProductProductTagProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductProductTagTable)
                .ForeignColumn("Product_Id")
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));
        }

        #endregion
    }
}