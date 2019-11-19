using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    /// <summary>
    /// Represents a product attribute combination mapping configuration
    /// </summary>
    public class AddProductAttributeCombinationProductFk : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ProductAttributeCombination))
                .ForeignColumn(nameof(ProductAttributeCombination.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));
        }

        #endregion
    }
}