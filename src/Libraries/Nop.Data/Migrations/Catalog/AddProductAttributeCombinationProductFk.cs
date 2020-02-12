using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    /// <summary>
    /// Represents a product attribute combination mapping configuration
    /// </summary>
    [NopMigration("2019/12/16 04:37:40:0830411")]
    public class AddProductAttributeCombinationProductFk : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductAttributeCombination),
                nameof(ProductAttributeCombination.ProductId),
                nameof(Product),
                nameof(Product.Id),
                Rule.Cascade);
        }

        #endregion
    }
}