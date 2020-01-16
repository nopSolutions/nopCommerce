using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097657438051844)]
    public class AddTierPriceProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(TierPrice), 
                nameof(TierPrice.ProductId), 
                nameof(Product), 
                nameof(Product.Id), 
                Rule.Cascade);
        }
        
        #endregion
    }
}