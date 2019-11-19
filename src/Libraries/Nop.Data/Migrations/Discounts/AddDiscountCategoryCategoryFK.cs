using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097771695936888)]
    public class AddDiscountCategoryCategoryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.DiscountAppliedToCategoriesTable)
                .ForeignColumn("Category_Id")
                .ToTable(nameof(Category))
                .PrimaryColumn(nameof(Category.Id));
        }

        #endregion
    }
}