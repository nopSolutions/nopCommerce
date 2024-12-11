using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.AmazonPay.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.UI;
using Nop.Web.Models.Customer;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Payments.AmazonPay.Services;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer :
    IConsumer<PageRenderingEvent>,
    IConsumer<ModelPreparedEvent<BaseNopModel>>,
    IConsumer<ModelReceivedEvent<BaseNopModel>>,
    IConsumer<CustomerAutoRegisteredByExternalMethodEvent>,
    IConsumer<ResetCheckoutDataEvent>
{
    #region Fields

    private readonly AmazonPayApiService _amazonPayApiService;
    private readonly AmazonPayCheckoutService _amazonPayCheckoutService;
    private readonly DisallowedProducts _disallowedProducts;
    private readonly ICategoryService _categoryService;
    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INopHtmlHelper _nopHtmlHelper;
    private readonly IPermissionService _permissionService;
    private readonly IProductService _productService;

    #endregion

    #region Ctor

    public EventConsumer(AmazonPayApiService amazonPayApiService,
        AmazonPayCheckoutService amazonPayCheckoutService,
        DisallowedProducts disallowedProducts,
        ICategoryService categoryService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        INopHtmlHelper nopHtmlHelper,
        IPermissionService permissionService,
        IProductService productService)
    {
        _amazonPayApiService = amazonPayApiService;
        _amazonPayCheckoutService = amazonPayCheckoutService;
        _disallowedProducts = disallowedProducts;
        _categoryService = categoryService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _nopHtmlHelper = nopHtmlHelper;
        _permissionService = permissionService;
        _productService = productService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle page rendering event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(PageRenderingEvent eventMessage)
    {
        if (!await _amazonPayApiService.IsActiveAndConfiguredAsync())
            return;

        var routeName = eventMessage.GetRouteName(true) ?? string.Empty;

        if (routeName == AmazonPayDefaults.CurrenciesPageRouteName)
            await _amazonPayApiService.EnsureCurrencyIsValidAsync();

        //add js script to one page checkout
        if (routeName == AmazonPayDefaults.OnePageCheckoutRouteName)
            _nopHtmlHelper.AddScriptParts(ResourceLocation.Footer, _amazonPayApiService.AmazonPayScriptUrl, excludeFromBundle: true);
    }

    /// <summary>
    /// Handle model prepared event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        var shoppingCartModel = eventMessage.Model as ShoppingCartModel;
        var customerAddressListModel = eventMessage.Model as CustomerAddressListModel;
        if (shoppingCartModel is null && customerAddressListModel is null)
            return;

        if (!await _amazonPayApiService.IsActiveAndConfiguredAsync())
            return;

        if (shoppingCartModel is not null)
        {
            var paymentDescriptor = await _amazonPayCheckoutService.GetPaymentDescriptorAsync();
            if (string.IsNullOrEmpty(paymentDescriptor) || !string.IsNullOrEmpty(shoppingCartModel.OrderReviewData.PaymentMethod))
                return;

            shoppingCartModel.OrderReviewData.PaymentMethod = paymentDescriptor;
        }

        if (customerAddressListModel is not null)
        {
            var addressIds = await _amazonPayCheckoutService.RemoveCustomerAddressesAsync();
            customerAddressListModel.Addresses = customerAddressListModel.Addresses
                .Where(addressModel => !addressIds.Contains(addressModel.Id))
                .ToList();
        }
    }

    /// <summary>
    /// Handle model received event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelReceivedEvent<BaseNopModel> eventMessage)
    {
        //get entity by received model
        var entity = eventMessage.Model switch
        {
            CategoryModel customerModel => (BaseEntity)await _categoryService.GetCategoryByIdAsync(customerModel.Id),
            ProductModel productModel => await _productService.GetProductByIdAsync(productModel.Id),
            _ => null
        };

        if (entity == null)
            return;

        if (!await _amazonPayApiService.IsActiveAndConfiguredAsync())
            return;

        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS))
            return;

        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
            return;

        //whether there is a form value for the custom value
        if (request.Form.TryGetValue(nameof(DoNotUseWithAmazonPayModel.DoNotUseWithAmazonPay),
                out var doNotUseWithAmazonPay)
            && !StringValues.IsNullOrEmpty(doNotUseWithAmazonPay))
            //save attribute
            if (bool.TryParse(doNotUseWithAmazonPay.FirstOrDefault(), out var flag))
            {
                await _genericAttributeService
                    .SaveAttributeAsync(entity, AmazonPayDefaults.DoNotUseWithAmazonPayAttributeName, flag);
                await _disallowedProducts.UpdateDataAsync(entity, flag);
            }
    }

    /// <summary>
    /// Handle external authentication event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(CustomerAutoRegisteredByExternalMethodEvent eventMessage)
    {
        if (eventMessage?.Customer == null || eventMessage.AuthenticationParameters == null)
            return;

        //handle event only for this authentication method
        if (!eventMessage.AuthenticationParameters.ProviderSystemName.Equals(AmazonPayDefaults.PluginSystemName))
            return;

        var customer = eventMessage.Customer;

        //store some of the customer fields
        var name = eventMessage.AuthenticationParameters.ExternalDisplayIdentifier.Split(' ');
        var firstName = name[0];
        var lastName = name.Length > 1 ? name[1] : string.Empty;

        if (!string.IsNullOrEmpty(firstName))
            customer.FirstName = firstName;

        if (!string.IsNullOrEmpty(lastName))
            customer.LastName = lastName;

        await _customerService.UpdateCustomerAsync(customer);
    }

    /// <summary>
    /// Handle reset checkout data event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ResetCheckoutDataEvent eventMessage)
    {
        if (!await _amazonPayApiService.IsActiveAndConfiguredAsync())
            return;

        await _amazonPayCheckoutService.RemoveCustomerAddressesAsync(eventMessage.Customer);
    }

    #endregion
}