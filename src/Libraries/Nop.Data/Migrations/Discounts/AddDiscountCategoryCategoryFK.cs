using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097771695936888)]
    public class AddDiscountCategoryCategoryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.DiscountAppliedToCategoriesTable
                , "Category_Id"
                , nameof(Category)
                , nameof(Category.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}