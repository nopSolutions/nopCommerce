using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:26:28:0193451")]
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