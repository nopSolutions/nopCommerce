using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Plugin.Misc.Omnisend.DTO.Events;
using Nop.Services.Common;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Represents the helper class for customer
/// </summary>
public class OmnisendCustomerService
{
    #region Fields

    private readonly IAddressService _addressService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IWebHelper _webHelper;
    private readonly OmnisendHttpClient _httpClient;

    #endregion

    #region Ctor

    public OmnisendCustomerService(IAddressService addressService,
        IGenericAttributeService genericAttributeService,
        INopUrlHelper nopUrlHelper,
        IWebHelper webHelper,
        OmnisendHttpClient httpClient)
    {
        _addressService = addressService;
        _genericAttributeService = genericAttributeService;
        _nopUrlHelper = nopUrlHelper;
        _webHelper = webHelper;
        _httpClient = httpClient;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the cart identifier for customer
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task<string> GetCartIdAsync(Customer customer)
    {
        var cartId = await _genericAttributeService.GetAttributeAsync<string>(customer,
            OmnisendDefaults.StoredCustomerShoppingCartIdAttribute);

        cartId = string.IsNullOrEmpty(cartId)
            ? await _genericAttributeService.GetAttributeAsync<string>(customer,
                OmnisendDefaults.CurrentCustomerShoppingCartIdAttribute)
            : cartId;

        if (!string.IsNullOrEmpty(cartId))
            return cartId;

        cartId = Guid.NewGuid().ToString();

        await _genericAttributeService.SaveAttributeAsync(customer,
            OmnisendDefaults.CurrentCustomerShoppingCartIdAttribute, cartId);

        return cartId;
    }

    /// <summary>
    /// Gets the customer email address
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="billingAddressId">billing address identifier</param>
    public async Task<string> GetEmailAsync(Customer customer, int? billingAddressId = null)
    {
        var email = !string.IsNullOrEmpty(customer.Email)
            ? customer.Email
            : await _genericAttributeService.GetAttributeAsync<string>(customer,
                OmnisendDefaults.CustomerEmailAttribute);

        return !string.IsNullOrEmpty(email)
            ? email
            : (await _addressService.GetAddressByIdAsync((billingAddressId ?? customer.BillingAddressId) ?? 0))
            ?.Email;
    }

    /// <summary>
    /// Create customer event
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="properties">Event properties</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains customer event or null if customer email is not determinate
    /// </returns>
    public async Task<CustomerEvents> CreateCustomerEventAsync(Customer customer, IEventProperty properties)
    {
        var email = await GetEmailAsync(customer);

        if (string.IsNullOrEmpty(email))
            return null;

        var customerEvent = new CustomerEvents { Email = email, Properties = properties };

        return customerEvent;
    }

    /// <summary>
    /// Gets the contact identifier
    /// </summary>
    /// <param name="email">Email to determinate customer</param>
    public async Task<string> GetContactIdAsync(string email)
    {
        var url = $"{OmnisendDefaults.ContactsApiUrl}?email={email}&limit=1";

        var res = await _httpClient.PerformRequestAsync(url, httpMethod: HttpMethod.Get);

        if (string.IsNullOrEmpty(res))
            return null;

        var contact = JsonConvert
            .DeserializeAnonymousType(res, new { contacts = new[] { new { contactID = string.Empty } } })?.contacts
            .FirstOrDefault();

        return contact?.contactID;
    }

    /// <summary>
    /// Store the cart identifier
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task StoreCartIdAsync(Customer customer)
    {
        var cartId = await _genericAttributeService.GetAttributeAsync<string>(customer,
            OmnisendDefaults.StoredCustomerShoppingCartIdAttribute);

        if (string.IsNullOrEmpty(cartId))
            await _genericAttributeService.SaveAttributeAsync(customer,
                OmnisendDefaults.StoredCustomerShoppingCartIdAttribute, await GetCartIdAsync(customer));
    }

    /// <summary>
    /// Delete the current shopping cart identifier for customer
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task DeleteCurrentCustomerShoppingCartIdAsync(Customer customer)
    {
        await _genericAttributeService.SaveAttributeAsync<string>(customer,
            OmnisendDefaults.CurrentCustomerShoppingCartIdAttribute, null);
    }

    /// <summary>
    /// Specifies whether to send the delete shopping cart event
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the True if we need to sand delete events</returns>
    public async Task<bool> IsNeedToSendDeleteShoppingCartEventAsync(Customer customer)
    {
        return string.IsNullOrEmpty(await _genericAttributeService.GetAttributeAsync<string>(customer,
            OmnisendDefaults.StoredCustomerShoppingCartIdAttribute));
    }

    /// <summary>
    /// Delete the stored shopping cart identifier for customer
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task DeleteStoredCustomerShoppingCartIdAsync(Customer customer)
    {
        await _genericAttributeService.SaveAttributeAsync<string>(customer,
            OmnisendDefaults.StoredCustomerShoppingCartIdAttribute, null);
    }

    /// <summary>
    /// Gets the abandoned checkout url
    /// </summary>
    /// <param name="cartId">Cart identifier</param>
    /// <returns>The abandoned checkout url</returns>
    public string GetAbandonedCheckoutUrl(string cartId)
    {
        return _nopUrlHelper.RouteUrl(OmnisendDefaults.AbandonedCheckoutRouteName, new { cartId }, _webHelper.GetCurrentRequestProtocol());
    }

    #endregion
}