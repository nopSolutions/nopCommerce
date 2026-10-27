using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.PunchOut.Domain;
using Nop.Plugin.Misc.PunchOut.Domain.CXML;
using Nop.Services.Authentication;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Tax;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Misc.PunchOut.Services;

/// <summary>
/// Represents the service to manage PunchOut operations
/// </summary>
public class PunchOutService
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IAuthenticationService _authenticationService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductService _productService;
    protected readonly IRepository<GenericAttribute> _genericAttributeRepository;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly ITaxService _taxService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly PunchOutIdentityService _punchOutIdentityService;
    protected readonly PunchOutLogService _punchOutLogService;
    protected readonly PunchOutSettings _punchOutSettings;

    #endregion

    #region Ctor

    public PunchOutService(IAddressService addressService,
        IAuthenticationService authenticationService,
        ICurrencyService currencyService,
        ICountryService countryService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILogger logger,
        IPriceCalculationService priceCalculationService,
        IProductAttributeFormatter productAttributeFormatter,
        IProductService productService,
        IRepository<GenericAttribute> genericAttributeRepository,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        ITaxService taxService,
        IWebHelper webHelper,
        IWorkContext workContext,
        PunchOutIdentityService punchOutIdentityService,
        PunchOutLogService punchOutLogService,
        PunchOutSettings punchOutSettings)
    {
        _addressService = addressService;
        _authenticationService = authenticationService;
        _currencyService = currencyService;
        _countryService = countryService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _logger = logger;
        _priceCalculationService = priceCalculationService;
        _productAttributeFormatter = productAttributeFormatter;
        _productService = productService;
        _genericAttributeRepository = genericAttributeRepository;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _taxService = taxService;
        _webHelper = webHelper;
        _workContext = workContext;
        _punchOutIdentityService = punchOutIdentityService;
        _punchOutLogService = punchOutLogService;
        _punchOutSettings = punchOutSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Validates the sender of the PunchOut message
    /// </summary>
    /// <param name="identity">The identity of the sender</param>
    /// <param name="sharedSecret">The incoming shared secret</param>
    /// <param name="payloadId">The payload ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the XML string
    /// </returns>
    private async Task<string> ValidateSenderAsync(string identity, string sharedSecret, string payloadId)
    {
        var sender = await _punchOutIdentityService.GetPunchOutIdentityAsync(identity)
            ?? throw new NopException("Unknown PunchOut identity.");

        // client validation verification
        var storedIdentity = sender?.SharedSecretHash;
        var incomingSecret = sharedSecret;

        var incomingBytes = Encoding.UTF8.GetBytes(incomingSecret);
        var expectedBytes = Encoding.UTF8.GetBytes(storedIdentity);

        if (!CryptographicOperations.FixedTimeEquals(incomingBytes, expectedBytes))
        {
            var errorXml = PunchOutXmlBuilder.BuildErrorResponse(new PunchOutErrorResponse
            {
                StatusCode = "401",
                StatusText = "Authentication Failed",
                ErrorMessage = "Invalid shared secret"
            });

            await _punchOutLogService.LogAsync(new PunchOutLog
            {
                PayloadId = payloadId,
                MessageTypeId = (int)PunchOutMessageType.SetupRequest,
                DirectionId = (int)PunchOutDirection.Outbound,
                RawXml = errorXml,
                Error = "Invalid shared secret"
            });

            return errorXml;
        }

        return string.Empty;
    }

    /// <summary>
    /// Generates a secure random token for PunchOut session identification
    /// </summary>
    /// <param name="length">The length of the token to generate</param>
    /// <returns>The generated token</returns>
    private static string GenerateSecureToken(int length)
    {
        var bytes = new byte[length];
        using var random = new SecureRandomNumberGenerator();
        random.GetBytes(bytes);

        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_")[..length];
    }

    /// <summary>
    /// Creates a new customer for PunchOut session based on the provided email
    /// </summary>
    /// <param name="customerEmail">The email of the customer to create</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer
    /// </returns>
    private async Task<Customer> CreatePunchOutCustomerAsync(string customerEmail)
    {
        var customer = new Customer
        {
            Email = customerEmail,
            Username = customerEmail,
            Active = true,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _customerService.InsertCustomerAsync(customer);
        await _customerService.InsertCustomerPasswordAsync(new CustomerPassword
        {
            CustomerId = customer.Id,
            PasswordFormat = PasswordFormat.Clear,
            Password = Guid.NewGuid().ToString("N")
        });

        var role = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
        if (role != null)
        {
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = role.Id });
        }

        return customer;
    }

    /// <summary>
    /// Creates or updates a customer address based on PunchOut address data
    /// </summary>
    /// <param name="punchOutAddress">The PunchOut address data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the created or updated address
    /// </returns>
    private async Task<Address> CreateOrUpdateAddressAsync(PunchOutAddress punchOutAddress)
    {
        ArgumentNullException.ThrowIfNull(punchOutAddress);

        // Get country by ISO code
        int? countryId = null;
        int? stateProvinceId = null;

        if (!string.IsNullOrEmpty(punchOutAddress.Country))
        {
            var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(punchOutAddress.Country);
            countryId = country?.Id;
        }

        if (countryId.HasValue && !string.IsNullOrEmpty(punchOutAddress.State))
        {
            var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(punchOutAddress.State, countryId.Value);
            stateProvinceId = state?.Id;
        }

        var address = new Address
        {
            FirstName = ExtractFirstName(punchOutAddress.Name),
            LastName = ExtractLastName(punchOutAddress.Name),
            Email = punchOutAddress.Email,
            Company = punchOutAddress.Company,
            CountryId = countryId,
            StateProvinceId = stateProvinceId,
            City = punchOutAddress.City,
            Address1 = punchOutAddress.Address1,
            Address2 = punchOutAddress.Address2,
            ZipPostalCode = punchOutAddress.PostalCode,
            PhoneNumber = punchOutAddress.PhoneNumber,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _addressService.InsertAddressAsync(address);
        return address;
    }

    /// <summary>
    /// Extracts first name from full name
    /// </summary>
    /// <param name="fullName">The full name</param>
    /// <returns>The first name</returns>
    private static string ExtractFirstName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
            return string.Empty;

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    /// <summary>
    /// Extracts last name from full name
    /// </summary>
    /// <param name="fullName">The full name</param>
    /// <returns>The last name</returns>
    private static string ExtractLastName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
            return string.Empty;

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
    }

    /// <summary>
    /// Get session by identifier
    /// </summary>
    /// <param name="sessionId">Session id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the session
    /// </returns>
    private async Task<PunchOutSession> GetPunchOutSessionByIdAsync(string sessionId)
    {
        var key = _staticCacheManager.PrepareKeyForDefaultCache(PunchOutDefaults.SessionTokenCacheKey, sessionId);

        return await _staticCacheManager.GetAsync(key, () => Task.FromResult<PunchOutSession>(null));
    }

    /// <summary>
    /// Save punchout session data to customer attributes
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="session">PunchOut session</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the session token
    /// </returns>
    private async Task<string> SavePunchoutSessionAsync(Customer customer, PunchOutSession session)
    {
        var token = session.SessionId;
        var jsonSession = JsonConvert.SerializeObject(session);
        await _genericAttributeService.SaveAttributeAsync(customer, PunchOutDefaults.PunchOutSessionAttribute, jsonSession, session.StoreId);

        //save session data to cache for quick retrieval during the session
        var key = _staticCacheManager.PrepareKeyForDefaultCache(PunchOutDefaults.SessionTokenCacheKey, session.SessionId);
        await _staticCacheManager.SetAsync(key, session);

        return token;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the incoming PunchOutSetupRequest
    /// </summary>
    /// <param name="xml">The XML string representing the setup request</param>
    /// <param name="httpContext">The HTTP context</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the response XML
    /// </returns>
    public async Task<string> HandleSetupRequestAsync(string xml, HttpContext httpContext)
    {
        var request = PunchOutXmlBuilder.ParseSetupRequest(xml);
        var sessionId = GenerateSecureToken(PunchOutDefaults.TokenLength);

        try
        {
            // inbound log
            await _punchOutLogService.LogAsync(new PunchOutLog
            {
                SessionId = sessionId,
                BuyerCookie = request.BuyerCookie,
                Identity = request.Identity,
                PayloadId = request.PayloadId,
                MessageTypeId = (int)PunchOutMessageType.SetupRequest,
                DirectionId = (int)PunchOutDirection.Inbound,
                RawXml = xml,
                Url = httpContext.Request.Path,
                HttpMethod = httpContext.Request.Method
            });

            var validationError = await ValidateSenderAsync(request.Identity, request.SharedSecret, request.PayloadId);
            if (!string.IsNullOrEmpty(validationError))
                return validationError;

            // customer creation
            var contactEmail = string.IsNullOrEmpty(request.Contact)
                ? throw new NopException("Customer email not found.")
                : request.Contact;

            var customer = await _customerService.GetCustomerByEmailAsync(contactEmail)
                ?? await CreatePunchOutCustomerAsync(contactEmail);

            if (request.ShipTo != null)
            {
                if (string.IsNullOrEmpty(request.ShipTo.Email))
                    request.ShipTo.Email = customer.Email;

                if (customer.ShippingAddressId.HasValue)
                {
                    var currentShippingAddress = await _addressService.GetAddressByIdAsync(customer.ShippingAddressId.Value);
                    if (currentShippingAddress.ZipPostalCode != request.ShipTo.PostalCode)
                    {
                        var shipToAddress = await CreateOrUpdateAddressAsync(request.ShipTo);
                        if (customer.ShippingAddressId != shipToAddress.Id)
                        {
                            customer.ShippingAddressId = shipToAddress.Id;
                        }
                    }
                }
                else
                {
                    var shipToAddress = await CreateOrUpdateAddressAsync(request.ShipTo);
                    customer.ShippingAddressId = shipToAddress.Id;
                }

                await _customerService.UpdateCustomerAsync(customer);
            }

            //Restricted customer role check
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            if (customerRoleIds.Intersect(_punchOutSettings.RestrictedCustomerRoleIds).Any())
                throw new NopException("Access with current customer role denied.");

            // PunchOut session
            var session = new PunchOutSession
            {
                SessionId = sessionId,
                BuyerCookie = request.BuyerCookie,
                ReturnUrl = request.BrowserFormPostUrl,
                CustomerId = customer.Id,
                StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                IsActive = false,
                CreatedOnUtc = DateTime.UtcNow
            };

            await SavePunchoutSessionAsync(customer, session);

            var storeLocation = _webHelper.GetStoreLocation();
            var startUrl = $"{storeLocation}punchout/start?sessionId={sessionId}";
            var responseXml = PunchOutXmlBuilder.BuildSetupResponse(
                new PunchOutSetupResponse
                {
                    SessionId = sessionId,
                    StartPageUrl = startUrl
                });

            // outbound log
            await _punchOutLogService.LogAsync(new PunchOutLog
            {
                PayloadId = request.PayloadId,
                SessionId = sessionId,
                MessageTypeId = (int)PunchOutMessageType.SetupResponse,
                DirectionId = (int)PunchOutDirection.Outbound,
                RawXml = responseXml,
            });

            return responseXml;
        }
        catch (Exception ex)
        {
            await _punchOutLogService.LogAsync(new PunchOutLog
            {
                SessionId = sessionId,
                PayloadId = request.PayloadId,
                MessageTypeId = (int)PunchOutMessageType.SetupRequest,
                DirectionId = (int)PunchOutDirection.Inbound,
                RawXml = xml,
                Error = ex.ToString()
            });

            return PunchOutXmlBuilder.BuildErrorResponse(new PunchOutErrorResponse
            {
                StatusCode = "400",
                StatusText = "Bad Request",
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// Activates the PunchOut session and returns session details for the storefront to start the session
    /// </summary>
    /// <param name="sessionId">The session ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the PunchOut session details and associated customer information
    /// </returns>
    public async Task<PunchOutSessionStartResult> StartSessionAsync(string sessionId)
    {
        var session = await GetPunchOutSessionByIdAsync(sessionId)
            ?? throw new NopException("PunchOut session not found.");

        if (session != null && !string.IsNullOrEmpty(session.SessionId) && session.CustomerId != 0)
        {
            var customer = await _customerService.GetCustomerByIdAsync(session.CustomerId);

            session.IsActive = true;
            session.CreatedOnUtc = DateTime.UtcNow;

            await SavePunchoutSessionAsync(customer, session);

            return new PunchOutSessionStartResult
            {
                Session = session,
                Customer = customer
            };
        }
        return new PunchOutSessionStartResult();
    }

    /// <summary>
    /// Builds the PunchOut response with the PunchOutOrderMessage XML payload
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the PunchOutReturnResponse
    /// </returns>
    public async Task<string> BuildReturnResponseAsync()
    {
        var session = await GetPunchOutSessionAsync()
            ?? throw new NopException("PunchOut session not found.");

        var cxml = await BuildOrderMessageAsync(session);
        var html = PunchOutXmlBuilder.BuildAutoSubmitForm(session.ReturnUrl, cxml);

        return html;
    }

    /// <summary>
    /// Builds the PunchOutOrderMessage XML based on the current customer's shopping cart
    /// </summary>
    /// <param name="session">Session</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the response XML
    /// </returns>
    public async Task<string> BuildOrderMessageAsync(PunchOutSession session)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
        var model = new PunchOutOrderMessage
        {
            BuyerCookie = session.BuyerCookie
        };

        var total = 0m;

        foreach (var item in cart)
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
            if (product is null)
                continue;

            var currency = await _workContext.GetWorkingCurrencyAsync();

            var (finalPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(item, true);
            var (shoppingCartItemSubTotalWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, finalPrice);
            var priceValue = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemSubTotalWithDiscountBase, currency);
            total += priceValue * item.Quantity;

            var name = await _localizationService.GetLocalizedAsync(product, x => x.Name);
            var attributes = !string.IsNullOrEmpty(item.AttributesXml)
                ? await _productAttributeFormatter.FormatAttributesAsync(product, item.AttributesXml)
                : null;
            model.Items.Add(new PunchOutOrderItem
            {
                SupplierPartId = await _productService.FormatSkuAsync(product, item.AttributesXml),
                Description = $"{name}{(!string.IsNullOrEmpty(attributes) ? $"({attributes})" : null)}",
                Quantity = item.Quantity,
                UnitPrice = priceValue,
                CurrencyCode = currency.CurrencyCode
            });
        }

        model.Total = total;

        return PunchOutXmlBuilder.BuildPunchOutOrderMessage(model);
    }

    #region Session

    /// <summary>
    /// Checks if the current customer has an active PunchOut session
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result indicates whether the customer has an active PunchOut session
    /// </returns>
    public async Task<bool> IsPunchoutSessionAsync()
    {
        try
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            var jsonSession = await _genericAttributeService
                .GetAttributeAsync<string>(customer, PunchOutDefaults.PunchOutSessionAttribute, store.Id)
                ?? string.Empty;
            var session = JsonConvert.DeserializeObject<PunchOutSession>(jsonSession);

            return session?.IsActive ?? false;
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("PunchOut: error determining session state", ex);
            return false;
        }
    }

    /// <summary>
    /// Gets all saved punchout session data for the customer
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the PunchOut session details
    /// </returns>
    public async Task<PunchOutSession> GetPunchOutSessionAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        try
        {
            var jsonSession = await _genericAttributeService
                .GetAttributeAsync<string>(customer, PunchOutDefaults.PunchOutSessionAttribute, store.Id)
                ?? string.Empty;
            var session = JsonConvert.DeserializeObject<PunchOutSession>(jsonSession);

            if (session != null)
            {
                session.IsActive = session?.IsActive ?? false && !string.IsNullOrEmpty(session.SessionId);
                return session;
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("PunchOut: error retrieving saved punchout session for customer", ex, customer);
        }

        return null;
    }

    /// <summary>
    /// Gets all saved punchout session data for the all customers
    /// </summary>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of PunchOut sessions
    /// </returns>
    public async Task<IPagedList<PunchOutSession>> GetAllPunchOutSessionAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        try
        {
            var query =
                from ga in _genericAttributeRepository.Table
                where ga.Key == PunchOutDefaults.PunchOutSessionAttribute &&
                      ga.KeyGroup == nameof(Customer)
                select ga;

            //store
            if (storeId > 0)
                query = query.Where(ga => ga.StoreId == storeId);

            var jsonSessions = await query.ToPagedListAsync(pageIndex, pageSize);
            var sessions = new List<PunchOutSession>();

            foreach (var jsonSession in jsonSessions)
            {
                var session = JsonConvert.DeserializeObject<PunchOutSession>(jsonSession.Value ?? string.Empty);
                if (session != null)
                    sessions.Add(session);
            }

            //paging
            return new PagedList<PunchOutSession>(sessions, pageIndex, pageSize);
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("PunchOut: error retrieving saved punchout sessions", ex);
        }

        return null;
    }

    /// <summary>
    /// Clear all punchout session data for the customer
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    public async Task ClearPunchoutSessionDataAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        try
        {
            var jsonSession = await _genericAttributeService
                .GetAttributeAsync<string>(customer, PunchOutDefaults.PunchOutSessionAttribute, store.Id)
                ?? string.Empty;
            var session = JsonConvert.DeserializeObject<PunchOutSession>(jsonSession);

            await _genericAttributeService.SaveAttributeAsync<string>(customer, PunchOutDefaults.PunchOutSessionAttribute, null, store.Id);

            if (session != null && !string.IsNullOrEmpty(session.SessionId))
            {
                //clear cache
                var key = _staticCacheManager.PrepareKeyForDefaultCache(PunchOutDefaults.SessionTokenCacheKey, session.SessionId);
                await _staticCacheManager.RemoveAsync(key);

                //standard logout 
                await _authenticationService.SignOutAsync();
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("PunchOut: error clearing punchout session data for customer", ex, customer);
        }
    }

    #endregion

    #endregion
}
