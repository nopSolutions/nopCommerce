using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025/05/23 08:00:00", "Multiple wishlist")]
public class MultipleWishlistMigration : ForwardOnlyMigration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<CustomWishlist>();

        //add new column
        this.AddOrAlterColumnFor<ShoppingCartItem>(t => t.CustomWishlistId).AsInt32().Nullable().ForeignKey<CustomWishlist>();
    }

    #endregion
}
