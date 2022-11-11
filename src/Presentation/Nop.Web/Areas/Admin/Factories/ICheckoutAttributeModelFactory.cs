using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the checkout attribute model factory
    /// </summary>
    public partial interface ICheckoutAttributeModelFactory
    {
        /// <summary>
        /// Prepare checkout attribute search model
        /// </summary>
        /// <param name="searchModel">Checkout attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attribute search model
        /// </returns>
        Task<CheckoutAttributeSearchModel> PrepareCheckoutAttributeSearchModelAsync(CheckoutAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged checkout attribute list model
        /// </summary>
        /// <param name="searchModel">Checkout attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attribute list model
        /// </returns>
        Task<CheckoutAttributeListModel> PrepareCheckoutAttributeListModelAsync(CheckoutAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare checkout attribute model
        /// </summary>
        /// <param name="model">Checkout attribute model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attribute model
        /// </returns>
        Task<CheckoutAttributeModel> PrepareCheckoutAttributeModelAsync(CheckoutAttributeModel model,
            CheckoutAttribute checkoutAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged checkout attribute value list model
        /// </summary>
        /// <param name="searchModel">Checkout attribute value search model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attribute value list model
        /// </returns>
        Task<CheckoutAttributeValueListModel> PrepareCheckoutAttributeValueListModelAsync(CheckoutAttributeValueSearchModel searchModel,
            CheckoutAttribute checkoutAttribute);

        /// <summary>
        /// Prepare checkout attribute value model
        /// </summary>
        /// <param name="model">Checkout attribute value model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attribute value model
        /// </returns>
        Task<CheckoutAttributeValueModel> PrepareCheckoutAttributeValueModelAsync(CheckoutAttributeValueModel model,
            CheckoutAttribute checkoutAttribute, CheckoutAttributeValue checkoutAttributeValue, bool excludeProperties = false);
    }
}