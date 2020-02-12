using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Discounts
{
    [NopMigration("2019/11/19 04:19:29:5936888")]
    public class AddDiscountCategoryCategoryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.DiscountAppliedToCategoriesTable,
                "Category_Id",
                nameof(Category),
                nameof(Category.Id),
                Rule.Cascade);
        }

        #endregion
    }
}