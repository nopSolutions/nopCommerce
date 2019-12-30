using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097631880193451)]
    public class AddAddProductProductTagProductTagFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductProductTagTable,
                "ProductTag_Id",
                nameof(ProductTag),
                nameof(ProductTag.Id),
                Rule.Cascade);
        }

        #endregion
    }
}