using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the checkout attribute model factory
    /// </summary>
    public partial interface ICheckoutAttributeModelFactory
    {
        /// <summary>
        /// Prepare paged checkout attribute list model for the grid
        /// </summary>
        /// <param name="command">Pagination parameters</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareCheckoutAttributeListGridModel(DataSourceRequest command);

        /// <summary>
        /// Prepare checkout attribute model
        /// </summary>
        /// <param name="model">Checkout attribute model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Checkout attribute model</returns>
        CheckoutAttributeModel PrepareCheckoutAttributeModel(CheckoutAttributeModel model,
            CheckoutAttribute checkoutAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged checkout attribute value list model for the grid
        /// </summary>
        /// <param name="command">Pagination parameters</param>
        /// <param name="CheckoutAttribute">Checkout attribute</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareCheckoutAttributeValueListGridModel(DataSourceRequest command,
            CheckoutAttribute checkoutAttribute);

        /// <summary>
        /// Prepare checkout attribute value model
        /// </summary>
        /// <param name="model">Checkout attribute value model</param>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Checkout attribute value model</returns>
        CheckoutAttributeValueModel PrepareCheckoutAttributeValueModel(CheckoutAttributeValueModel model,
            CheckoutAttribute checkoutAttribute, CheckoutAttributeValue checkoutAttributeValue, bool excludeProperties = false);
    }
}