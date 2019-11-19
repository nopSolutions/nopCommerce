using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097631880193451)]
    public class AddAddProductProductTagProductTagFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductProductTagTable)
                .ForeignColumn("ProductTag_Id")
                .ToTable(nameof(ProductTag))
                .PrimaryColumn(nameof(ProductTag.Id));
        }

        #endregion
    }
}