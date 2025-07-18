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
        if (!Schema.Table(nameof(CustomWishlist)).Exists())
            Create.TableFor<CustomWishlist>();

        //add new column
        if (!Schema.Table(nameof(ShoppingCartItem)).Column(nameof(ShoppingCartItem.CustomWishlistId)).Exists())
            Alter.Table(nameof(ShoppingCartItem)).AddColumn(nameof(ShoppingCartItem.CustomWishlistId)).AsInt32().Nullable().ForeignKey<CustomWishlist>();
    }

    #endregion
}
