using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Web.Areas.Admin.Models.ShoppingCart;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the shopping cart model factory
    /// </summary>
    public partial interface IShoppingCartModelFactory
    {
        /// <summary>
        /// Prepare shopping cart search model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>Shopping cart search model</returns>
        Task<ShoppingCartSearchModel> PrepareShoppingCartSearchModelAsync(ShoppingCartSearchModel searchModel);

        /// <summary>
        /// Prepare paged shopping cart list model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>Shopping cart list model</returns>
        Task<ShoppingCartListModel> PrepareShoppingCartListModelAsync(ShoppingCartSearchModel searchModel);

        /// <summary>
        /// Prepare paged shopping cart item list model
        /// </summary>
        /// <param name="searchModel">Shopping cart item search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shopping cart item list model</returns>
        Task<ShoppingCartItemListModel> PrepareShoppingCartItemListModelAsync(ShoppingCartItemSearchModel searchModel, Customer customer);
    }
}