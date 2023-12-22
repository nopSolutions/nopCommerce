using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.Customer;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the interface of the customer model factory
/// </summary>
public partial interface ICustomerModelFactory
{
    /// <summary>
    /// Prepare the customer info model
    /// </summary>
    /// <param name="model">Customer info model</param>
    /// <param name="customer">Customer</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer info model
    /// </returns>
    Task<CustomerInfoModel> PrepareCustomerInfoModelAsync(CustomerInfoModel model, Customer customer,
        bool excludeProperties, string overrideCustomCustomerAttributesXml = "");

    /// <summary>
    /// Prepare the customer register model
    /// </summary>
    /// <param name="model">Customer register model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer register model
    /// </returns>
    Task<RegisterModel> PrepareRegisterModelAsync(RegisterModel model, bool excludeProperties,
        string overrideCustomCustomerAttributesXml = "", bool setDefaultValues = false);

    /// <summary>
    /// Prepare the login model
    /// </summary>
    /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the login model
    /// </returns>
    Task<LoginModel> PrepareLoginModelAsync(bool? checkoutAsGuest);

    /// <summary>
    /// Prepare the password recovery model
    /// </summary>
    /// <param name="model">Password recovery model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the password recovery model
    /// </returns>
    Task<PasswordRecoveryModel> PreparePasswordRecoveryModelAsync(PasswordRecoveryModel model);

    /// <summary>
    /// Prepare the register result model
    /// </summary>
    /// <param name="resultId">Value of UserRegistrationType enum</param>
    /// <param name="returnUrl">URL to redirect</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the register result model
    /// </returns>
    Task<RegisterResultModel> PrepareRegisterResultModelAsync(int resultId, string returnUrl);

    /// <summary>
    /// Prepare the customer navigation model
    /// </summary>
    /// <param name="selectedTabId">Identifier of the selected tab</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer navigation model
    /// </returns>
    Task<CustomerNavigationModel> PrepareCustomerNavigationModelAsync(int selectedTabId = 0);

    /// <summary>
    /// Prepare the customer address list model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address list model  
    /// </returns>
    Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync();

    /// <summary>
    /// Prepare the customer downloadable products model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer downloadable products model
    /// </returns>
    Task<CustomerDownloadableProductsModel> PrepareCustomerDownloadableProductsModelAsync();

    /// <summary>
    /// Prepare the user agreement model
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the user agreement model
    /// </returns>
    Task<UserAgreementModel> PrepareUserAgreementModelAsync(OrderItem orderItem, Product product);

    /// <summary>
    /// Prepare the change password model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the change password model
    /// </returns>
    Task<ChangePasswordModel> PrepareChangePasswordModelAsync();

    /// <summary>
    /// Prepare the customer avatar model
    /// </summary>
    /// <param name="model">Customer avatar model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer avatar model
    /// </returns>
    Task<CustomerAvatarModel> PrepareCustomerAvatarModelAsync(CustomerAvatarModel model);

    /// <summary>
    /// Prepare the GDPR tools model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR tools model
    /// </returns>
    Task<GdprToolsModel> PrepareGdprToolsModelAsync();

    /// <summary>
    /// Prepare the check gift card balance model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check gift card balance model
    /// </returns>
    Task<CheckGiftCardBalanceModel> PrepareCheckGiftCardBalanceModelAsync();

    /// <summary>
    /// Prepare the multi-factor authentication model
    /// </summary>
    /// <param name="model">Multi-factor authentication model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multi-factor authentication model
    /// </returns>
    Task<MultiFactorAuthenticationModel> PrepareMultiFactorAuthenticationModelAsync(MultiFactorAuthenticationModel model);

    /// <summary>
    /// Prepare the multi-factor provider model
    /// </summary>
    /// <param name="providerModel">Multi-factor provider model</param>
    /// <param name="sysName">Multi-factor provider system name</param>
    /// <param name="isLogin">Is login page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multi-factor authentication model
    /// </returns>
    Task<MultiFactorAuthenticationProviderModel> PrepareMultiFactorAuthenticationProviderModelAsync(MultiFactorAuthenticationProviderModel providerModel, string sysName, bool isLogin = false);

    /// <summary>
    /// Prepare the custom customer attribute models
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="overrideAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the customer attribute model
    /// </returns>
    Task<IList<CustomerAttributeModel>> PrepareCustomCustomerAttributesAsync(Customer customer, string overrideAttributesXml = "");
}