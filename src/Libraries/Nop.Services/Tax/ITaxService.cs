using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;

namespace Nop.Services.Tax;

/// <summary>
/// Tax service
/// </summary>
public partial interface ITaxService
{
    #region Product price

    /// <summary>
    /// Gets price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="price">Price</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price);

    /// <summary>
    /// Gets price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="price">Price</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price, Customer customer);

    /// <summary>
    /// Gets price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="price">Price</param>
    /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price,
        bool includingTax, Customer customer);

    /// <summary>
    /// Gets price
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="taxCategoryId">Tax category identifier</param>
    /// <param name="price">Price</param>
    /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
    /// <param name="customer">Customer</param>
    /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, int taxCategoryId, decimal price,
        bool includingTax, Customer customer,
        bool priceIncludesTax);

    /// <summary>
    /// Gets a value indicating whether a product is tax exempt
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether a product is tax exempt
    /// </returns>
    Task<bool> IsTaxExemptAsync(Product product, Customer customer);

    #endregion

    #region Shipping price

    /// <summary>
    /// Gets shipping price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, Customer customer);

    /// <summary>
    /// Gets shipping price
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, bool includingTax, Customer customer);

    #endregion

    #region Payment additional fee

    /// <summary>
    /// Gets payment method additional handling fee
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, Customer customer);

    /// <summary>
    /// Gets payment method additional handling fee
    /// </summary>
    /// <param name="price">Price</param>
    /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, bool includingTax, Customer customer);

    #endregion

    #region Checkout attribute price

    /// <summary>
    /// Gets checkout attribute value price
    /// </summary>
    /// <param name="ca">Checkout attribute</param>
    /// <param name="cav">Checkout attribute value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav);

    /// <summary>
    /// Gets checkout attribute value price
    /// </summary>
    /// <param name="ca">Checkout attribute</param>
    /// <param name="cav">Checkout attribute value</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav, Customer customer);

    /// <summary>
    /// Gets checkout attribute value price
    /// </summary>
    /// <param name="ca">Checkout attribute</param>
    /// <param name="cav">Checkout attribute value</param>
    /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price. Tax rate
    /// </returns>
    Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav,
        bool includingTax, Customer customer);

    #endregion

    #region VAT

    /// <summary>
    /// Gets VAT Number status
    /// </summary>
    /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vAT Number status
    /// </returns>
    Task<(VatNumberStatus vatNumberStatus, string name, string address)> GetVatNumberStatusAsync(string fullVatNumber);

    #endregion

    #region Tax total

    /// <summary>
    /// Get tax total for the passed shopping cart
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<TaxTotalResult> GetTaxTotalAsync(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true);

    #endregion
}