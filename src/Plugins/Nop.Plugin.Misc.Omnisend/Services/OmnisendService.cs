using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Tax;

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Represents the main plugin service class
/// </summary>
public class OmnisendService
{
    #region Fields

    private readonly ICategoryService _categoryService;
    private readonly ICountryService _countryService;
    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductService _productService;
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<NewsLetterSubscription> _newsLetterSubscriptionRepository;
    private readonly IRepository<StateProvince> _stateProvinceRepository;
    private readonly ISettingService _settingService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly IStoreContext _storeContext;
    private readonly ITaxService _taxService;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;
    private readonly OmnisendCustomerService _omnisendCustomerService;
    private readonly OmnisendHelper _omnisendHelper;
    private readonly OmnisendHttpClient _omnisendHttpClient;
    private readonly OmnisendSettings _omnisendSettings;

    #endregion

    #region Ctor

    public OmnisendService(ICategoryService categoryService,
        ICountryService countryService,
        ICustomerService customerService,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IRepository<Country> countryRepository,
        IRepository<Customer> customerRepository,
        IRepository<NewsLetterSubscription> newsLetterSubscriptionRepository,
        IRepository<StateProvince> stateProvinceRepository,
        ISettingService settingService,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        ITaxService taxService,
        IWebHelper webHelper,
        IWorkContext workContext,
        OmnisendCustomerService omnisendCustomerService,
        OmnisendHelper omnisendHelper,
        OmnisendHttpClient omnisendHttpClient,
        OmnisendSettings omnisendSettings)
    {
        _categoryService = categoryService;
        _countryService = countryService;
        _customerService = customerService;
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _countryRepository = countryRepository;
        _customerRepository = customerRepository;
        _newsLetterSubscriptionRepository = newsLetterSubscriptionRepository;
        _stateProvinceRepository = stateProvinceRepository;
        _settingService = settingService;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _taxService = taxService;
        _webHelper = webHelper;
        _workContext = workContext;
        _omnisendCustomerService = omnisendCustomerService;
        _omnisendHelper = omnisendHelper;
        _omnisendHttpClient = omnisendHttpClient;
        _omnisendSettings = omnisendSettings;
    }

    #endregion

    #region Utilities

    private async Task FillCustomerInfoAsync(BaseContactInfoDto dto, Customer customer)
    {
        if (customer == null)
            return;

        dto.FirstName = customer.FirstName;
        dto.LastName = customer.LastName;

        var country = await _countryService.GetCountryByIdAsync(customer.CountryId);

        if (country != null)
        {
            dto.Country = country.Name;
            dto.CountryCode = country.TwoLetterIsoCode;
        }

        var state = await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId);

        if (state != null)
            dto.State = state.Name;

        dto.City = customer.City;
        dto.Address = customer.StreetAddress;
        dto.PostalCode = customer.ZipPostalCode;
        dto.Gender = customer.Gender?.ToLower() ?? "f";
        dto.BirthDate = customer.DateOfBirth?.ToString("yyyy-MM-dd");
    }

    private async Task<OrderDto.OrderItemDto> OrderItemToDtoAsync(OrderItem orderItem)
    {
        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

        var (sku, variantId) = await _omnisendHelper.GetSkuAndVariantIdAsync(product, orderItem.AttributesXml);

        var dto = new OrderDto.OrderItemDto
        {
            ProductId = orderItem.ProductId.ToString(),
            Sku = sku,
            VariantId = variantId.ToString(),
            Title = product.Name,
            Quantity = orderItem.Quantity,
            Price = orderItem.PriceInclTax.ToCents()
        };

        return dto;
    }

    private async Task<ProductDto> ProductToDtoAsync(Product product)
    {
        async Task<List<string>> getProductCategories()
        {
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);

            return productCategories.Select(pc => pc.CategoryId.ToString()).ToList();
        }

        async Task<IList<ProductAttributeCombination>> getProductCombinations()
        {
            return await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
        }

        var combinations = await getProductCombinations();

        async Task<string> getProductStatus(ProductAttributeCombination productAttributeCombination = null)
        {
            var status = "notAvailable";

            if (!product.Published || product.Deleted)
                return status;

            int stockQuantity;

            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    stockQuantity = await _productService.GetTotalStockQuantityAsync(product);

                    if (stockQuantity > 0 || product.BackorderMode == BackorderMode.AllowQtyBelow0)
                        status = "inStock";
                    else
                        status = "outOfStock";

                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    if (productAttributeCombination == null)
                        return combinations.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders) ? "inStock" : "outOfStock";

                    stockQuantity = productAttributeCombination.StockQuantity;

                    if (stockQuantity > 0 || productAttributeCombination.AllowOutOfStockOrders)
                        status = "inStock";
                    else
                        status = "outOfStock";

                    break;
                case ManageInventoryMethod.DontManageStock:
                    status = "inStock";
                    break;
            }

            return status;
        }

        var dto = new ProductDto
        {
            ProductId = product.Id.ToString(),
            Title = product.Name,
            Status = await getProductStatus(),
            Description = product.ShortDescription,
            Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            ProductUrl = await _omnisendHelper.GetProductUrlAsync(product),
            Images = new List<ProductDto.Image>
            {
                await _omnisendHelper.GetProductPictureUrlAsync(product),
            },
            CreatedAt = product.CreatedOnUtc.ToDtoString(),
            UpdatedAt = product.UpdatedOnUtc.ToDtoString(),
            CategoryIDs = await getProductCategories(),
            Variants = new List<ProductDto.Variant>
            {
                new()
                {
                    VariantId = product.Id.ToString(),
                    Title = product.Name,
                    Sku = product.Sku,
                    Status = await getProductStatus(),
                    Price = product.Price.ToCents()
                }
            }
        };

        if (combinations.Any())
            dto.Variants.AddRange(await combinations.SelectAwait(async c => new ProductDto.Variant
            {
                VariantId = c.Id.ToString(),
                Title = product.Name,
                Sku = c.Sku,
                Status = await getProductStatus(c),
                Price = (c.OverriddenPrice ?? product.Price).ToCents()
            }).ToListAsync());

        return dto;
    }

    private async Task<OrderDto> OrderToDtoAsync(Order order)
    {
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
        var email = await _omnisendCustomerService.GetEmailAsync(customer, order.BillingAddressId);
        if (string.IsNullOrEmpty(email))
            return null;

        var dto = new OrderDto
        {
            OrderId = order.OrderGuid.ToString(),
            Email = email,
            Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            OrderSum = order.OrderTotal.ToCents(),
            SubTotalSum = order.OrderSubtotalInclTax.ToCents(),
            DiscountSum = order.OrderDiscount.ToCents(),
            TaxSum = order.OrderTax.ToCents(),
            ShippingSum = order.OrderShippingInclTax.ToCents(),
            CreatedAt = order.CreatedOnUtc.ToDtoString(),
            Products = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async oi => await OrderItemToDtoAsync(oi)).ToListAsync()
        };

        return dto;
    }

    private async Task<CartItemDto> ShoppingCartItemToDtoAsync(ShoppingCartItem shoppingCartItem, Customer customer)
    {
        var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

        var (sku, variantId) = await _omnisendHelper.GetSkuAndVariantIdAsync(product, shoppingCartItem.AttributesXml);

        var (scSubTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(shoppingCartItem, true);

        var dto = new CartItemDto
        {
            CartProductId = shoppingCartItem.Id.ToString(),
            ProductId = shoppingCartItem.ProductId.ToString(),
            Sku = sku,
            VariantId = variantId.ToString(),
            Title = product.Name,
            Quantity = shoppingCartItem.Quantity,
            Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            Price = (await _taxService.GetProductPriceAsync(product, scSubTotal, true, customer)).price.ToCents()
        };

        return dto;
    }

    private CategoryDto CategoryToDto(Category category)
    {
        return new CategoryDto
        {
            CategoryId = category.Id.ToString(),
            Title = category.Name,
            CreatedAt = category.CreatedOnUtc.ToDtoString()
        };
    }

    private async Task<CartDto> GetCartDtoAsync(IList<ShoppingCartItem> cart)
    {
        if (!cart.Any())
            return null;

        var customerId = cart.First().CustomerId;
        var customer = await _customerService.GetCustomerByIdAsync(customerId);

        var items = await cart
            .SelectAwait(async ci => await ShoppingCartItemToDtoAsync(ci, customer))
            .ToListAsync();

        var cartSum = (await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal
            ?.ToCents() ?? 0;

        var email = await _omnisendCustomerService.GetEmailAsync(customer);
        if (string.IsNullOrEmpty(email))
            return null;

        var cartId = await _omnisendCustomerService.GetCartIdAsync(customer);

        return new CartDto
        {
            Currency = await _omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            Email = email,
            CartId = cartId,
            CartSum = cartSum,
            Products = items,
            CartRecoveryUrl = _omnisendCustomerService.GetAbandonedCheckoutUrl(cartId)
        };
    }

    private async Task CreateOrderAsync(Order order)
    {
        var orderDto = await OrderToDtoAsync(order);
        if (orderDto is null)
            return;

        var data = JsonConvert.SerializeObject(orderDto);
        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.OrdersApiUrl, data, HttpMethod.Post);
    }

    private async Task CreateCartAsync(IList<ShoppingCartItem> cart)
    {
        var cartDto = await GetCartDtoAsync(cart);
        if (cartDto is null)
            return;

        var data = JsonConvert.SerializeObject(cartDto);
        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CartsApiUrl, data, HttpMethod.Post);
    }

    private async Task UpdateCartAsync(Customer customer, ShoppingCartItem newItem)
    {
        var item = await ShoppingCartItemToDtoAsync(newItem, customer);
        if (item is null)
            return;

        var data = JsonConvert.SerializeObject(item);
        await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}", data, HttpMethod.Post);
    }

    private async Task<bool> CanSendRequestAsync(Customer customer)
    {
        return !string.IsNullOrEmpty(
            await _omnisendCustomerService.GetEmailAsync(customer, customer.BillingAddressId));
    }

    /// <summary>
    /// Prepare newsletter subscribers to sync
    /// </summary>
    /// <param name="storeId">Store identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sendWelcomeMessage">Specifies whether to send a welcome message</param>
    /// <param name="subscriber">Newsletter subscription to filter</param>
    /// <param name="inactiveStatus">Inactive status</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of subscriber data
    /// </returns>
    private async Task<List<IBatchSupport>> PrepareNewsletterSubscribersAsync(int storeId,
        int pageIndex, int pageSize,
        bool sendWelcomeMessage = false, NewsLetterSubscription subscriber = null, string inactiveStatus = "nonSubscribed")
    {
        //get contacts of newsletter subscribers
        var subscriptions = (subscriber == null ? _newsLetterSubscriptionRepository.Table : _newsLetterSubscriptionRepository.Table.Where(nlsr => nlsr.Id.Equals(subscriber.Id)))
            .Where(subscription => subscription.StoreId == storeId)
            .OrderBy(subscription => subscription.Id)
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

        var contacts = from item in subscriptions
            join c in _customerRepository.Table on item.Email equals c.Email
                into temp
            from c in temp.DefaultIfEmpty()
            where c == null || (c.Active && !c.Deleted)
            select new { subscription = item, customer = c };

        var contactsWithCountry = from item in contacts
            join cr in _countryRepository.Table on item.customer.CountryId equals cr.Id
                into temp
            from cr in temp.DefaultIfEmpty()
            select new { item.customer, item.subscription, country = cr };

        var contactsWithState = from item in contactsWithCountry
            join sp in _stateProvinceRepository.Table on item.customer.StateProvinceId equals sp.Id
                into temp
            from sp in temp.DefaultIfEmpty()
            select new
            {
                item.subscription,
                item.customer.FirstName,
                item.customer.LastName,
                CountryName = item.country.Name,
                CountryTwoLetterIsoCode = item.country.TwoLetterIsoCode,
                StateProvinceName = sp.Name,
                item.customer.City,
                item.customer.StreetAddress,
                item.customer.ZipPostalCode,
                item.customer.Gender,
                item.customer.DateOfBirth
            };

        var subscribers = (await contactsWithState.ToListAsync()).Select(item =>
        {
            var dto = new CreateContactRequest(item.subscription, inactiveStatus, sendWelcomeMessage)
            {
                FirstName = item.FirstName,
                LastName = item.LastName,
                City = item.City,
                Address = item.StreetAddress,
                PostalCode = item.ZipPostalCode,
                Gender = item.Gender?.ToLower(),
                BirthDate = item.DateOfBirth?.ToString("yyyy-MM-dd")
            };

            if (!string.IsNullOrEmpty(item.CountryName))
                dto.Country = item.CountryName;

            if (!string.IsNullOrEmpty(item.CountryTwoLetterIsoCode))
                dto.CountryCode = item.CountryTwoLetterIsoCode;

            if (!string.IsNullOrEmpty(item.StateProvinceName))
                dto.State = item.StateProvinceName;

            return (IBatchSupport)dto;
        }).ToList();

        return subscribers;
    }

    #endregion

    #region Methods

    #region Sync methods

    /// <summary>
    /// Synchronize contacts
    /// </summary>
    public async Task SyncContactsAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var su = await PrepareNewsletterSubscribersAsync(store.Id, 0, _omnisendSettings.PageSize);

        if (su.Count >= OmnisendDefaults.MinCountToUseBatch)
        {
            var page = 0;

            while (true)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
                    Endpoint = OmnisendDefaults.ContactsEndpoint,
                    Items = su
                });

                var rez = await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

                var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

                if (bathId != null)
                {
                    _omnisendSettings.BatchesIds.Add(bathId);
                    await _settingService.SaveSettingAsync(_omnisendSettings);
                }

                page++;

                su = await PrepareNewsletterSubscribersAsync(store.Id, page, _omnisendSettings.PageSize);

                if (!su.Any())
                    break;
            }
        }
        else
            foreach (var newsLetterSubscription in su)
                await UpdateOrCreateContactAsync(newsLetterSubscription as CreateContactRequest);
    }

    /// <summary>
    /// Synchronize categories
    /// </summary>
    public async Task SyncCategoriesAsync()
    {
        var categories = await _categoryService.GetAllCategoriesAsync(null, pageSize: _omnisendSettings.PageSize);

        if (categories.TotalCount >= OmnisendDefaults.MinCountToUseBatch || categories.TotalCount > _omnisendSettings.PageSize)
        {
            var page = 0;

            while (page < categories.TotalPages)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
                    Endpoint = OmnisendDefaults.CategoriesEndpoint,
                    Items = categories.Select(category => CategoryToDto(category) as IBatchSupport).ToList()
                });

                var rez = await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

                var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

                if (bathId != null)
                {
                    _omnisendSettings.BatchesIds.Add(bathId);
                    await _settingService.SaveSettingAsync(_omnisendSettings);
                }

                page++;

                categories = await _categoryService.GetAllCategoriesAsync(null, pageIndex: page, pageSize: _omnisendSettings.PageSize);
            }
        }
        else
            foreach (var category in categories)
            {
                var data = JsonConvert.SerializeObject(CategoryToDto(category));
                await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CategoriesApiUrl, data, HttpMethod.Post);
            }
    }

    /// <summary>
    /// Synchronize products
    /// </summary>
    public async Task SyncProductsAsync()
    {
        var products = await _productService.SearchProductsAsync(pageSize: _omnisendSettings.PageSize);

        if (products.TotalCount >= OmnisendDefaults.MinCountToUseBatch || products.TotalCount > _omnisendSettings.PageSize)
        {
            var page = 0;

            while (page < products.TotalPages)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
                    Endpoint = OmnisendDefaults.ProductsEndpoint,
                    Items = await products.SelectAwait(async product => await ProductToDtoAsync(product) as IBatchSupport).ToListAsync()
                });

                var rez = await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

                var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

                if (bathId != null)
                {
                    _omnisendSettings.BatchesIds.Add(bathId);
                    await _settingService.SaveSettingAsync(_omnisendSettings);
                }

                page++;

                products = await _productService.SearchProductsAsync(pageIndex: page, pageSize: _omnisendSettings.PageSize);
            }
        }
        else
            foreach (var product in products)
                await AddNewProductAsync(product);
    }

    /// <summary>
    /// Synchronize orders
    /// </summary>
    public async Task SyncOrdersAsync()
    {
        var orders = await _orderService.SearchOrdersAsync(pageSize: _omnisendSettings.PageSize);

        if (orders.TotalCount >= OmnisendDefaults.MinCountToUseBatch || orders.TotalCount > _omnisendSettings.PageSize)
        {
            var page = 0;

            while (page < orders.TotalPages)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
                    Endpoint = OmnisendDefaults.OrdersEndpoint,
                    Items = await orders.SelectAwait(async order => await OrderToDtoAsync(order) as IBatchSupport).ToListAsync()
                });

                var rez = await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

                var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

                if (bathId != null)
                {
                    _omnisendSettings.BatchesIds.Add(bathId);
                    await _settingService.SaveSettingAsync(_omnisendSettings);
                }

                page++;

                orders = await _orderService.SearchOrdersAsync(pageIndex: page, pageSize: _omnisendSettings.PageSize);
            }
        }
        else
            foreach (var order in orders)
                await CreateOrderAsync(order);
    }

    /// <summary>
    /// Synchronize shopping carts
    /// </summary>
    public async Task SyncCartsAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var customers = await _customerService.GetCustomersWithShoppingCartsAsync(ShoppingCartType.ShoppingCart, store.Id);
        foreach (var customer in customers)
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            await CreateCartAsync(cart);
        }
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Gets the brand identifier 
    /// </summary>
    /// <param name="apiKey">API key to send request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the brand identifier or null
    /// </returns>
    public async Task<string> GetBrandIdAsync(string apiKey)
    {
        _omnisendHttpClient.ApiKey = apiKey;

        var result = await _omnisendHttpClient.PerformRequestAsync<AccountsResponse>(OmnisendDefaults.AccountsApiUrl);

        return result?.BrandId;
    }

    /// <summary>
    /// Registers the site on the omnisend service
    /// </summary>
    public async Task SendCustomerDataAsync()
    {
        var site = _webHelper.GetStoreLocation();

        var data = JsonConvert.SerializeObject(new
        {
            website = site,
            platform = OmnisendDefaults.IntegrationOrigin,
            version = OmnisendDefaults.IntegrationVersion
        });

        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.AccountsApiUrl, data, HttpMethod.Post);
    }

    /// <summary>
    /// Gets information about batch
    /// </summary>
    /// <param name="bathId">Batch identifier</param>
    public async ValueTask<BatchResponse> GetBatchInfoAsync(string bathId)
    {
        var url = OmnisendDefaults.BatchesApiUrl + $"/{bathId}";

        var result = await _omnisendHttpClient.PerformRequestAsync<BatchResponse>(url, httpMethod: HttpMethod.Get);

        return result;
    }

    #endregion

    #region Contacts

    /// <summary>
    /// Gets the contacts information
    /// </summary>
    /// <param name="contactId">Contact identifier</param>
    public async Task<ContactInfoDto> GetContactInfoAsync(string contactId)
    {
        var url = $"{OmnisendDefaults.ContactsApiUrl}/{contactId}";

        var res = await _omnisendHttpClient.PerformRequestAsync<ContactInfoDto>(url, httpMethod: HttpMethod.Get);

        return res;
    }

    /// <summary>
    /// Update or create contact information
    /// </summary>
    /// <param name="request">Create contact request</param>
    public async Task UpdateOrCreateContactAsync(CreateContactRequest request)
    {
        var email = request.Identifiers.First().Id;
        var exists = !string.IsNullOrEmpty(await _omnisendCustomerService.GetContactIdAsync(email));

        if (!exists)
        {
            var data = JsonConvert.SerializeObject(request);
            await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.ContactsApiUrl, data, HttpMethod.Post);
        }
        else
        {
            var url = $"{OmnisendDefaults.ContactsApiUrl}?email={email}";
            var data = JsonConvert.SerializeObject(new { identifiers = new[] { request.Identifiers.First() } });

            await _omnisendHttpClient.PerformRequestAsync(url, data, HttpMethod.Patch);
        }
    }

    /// <summary>
    /// Update or create contact information
    /// </summary>
    /// <param name="subscription">Newsletter subscription</param>
    /// <param name="sendWelcomeMessage">Specifies whether to send a welcome message</param>
    public async Task UpdateOrCreateContactAsync(NewsLetterSubscription subscription, bool sendWelcomeMessage = false)
    {
        var su = await PrepareNewsletterSubscribersAsync(subscription.StoreId,
            0, _omnisendSettings.PageSize,
            sendWelcomeMessage, subscription, "unsubscribed");

        if (su.FirstOrDefault() is not CreateContactRequest request)
            return;

        await UpdateOrCreateContactAsync(request);
    }

    /// <summary>
    /// Updates contact information by customer data
    /// </summary>
    /// <param name="customer">Customer</param>
    public async Task UpdateContactAsync(Customer customer)
    {
        var contactId = await _omnisendCustomerService.GetContactIdAsync(customer.Email);

        if (string.IsNullOrEmpty(contactId))
            return;

        var url = $"{OmnisendDefaults.ContactsApiUrl}/{contactId}";

        var dto = new BaseContactInfoDto();

        await FillCustomerInfoAsync(dto, customer);

        var data = JsonConvert.SerializeObject(dto);

        await _omnisendHttpClient.PerformRequestAsync(url, data, HttpMethod.Patch);
    }

    #endregion

    #region Products

    /// <summary>
    /// Adds new product
    /// </summary>
    /// <param name="product">Product to add</param>
    public async Task AddNewProductAsync(Product product)
    {
        var data = JsonConvert.SerializeObject(await ProductToDtoAsync(product));
        await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.ProductsApiUrl, data, HttpMethod.Post);
    }

    /// <summary>
    /// Updates the product
    /// </summary>
    /// <param name="productId">Product identifier to update</param>
    public async Task UpdateProductAsync(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);

        await CreateOrUpdateProductAsync(product);
    }

    /// <summary>
    /// Updates the product
    /// </summary>
    /// <param name="product">Product to update</param>
    public async Task CreateOrUpdateProductAsync(Product product)
    {
        var result = await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.ProductsApiUrl}/{product.Id}", httpMethod: HttpMethod.Get);
        if (string.IsNullOrEmpty(result))
            await AddNewProductAsync(product);
        else
        {
            var data = JsonConvert.SerializeObject(await ProductToDtoAsync(product));
            await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.ProductsApiUrl}/{product.Id}", data, HttpMethod.Put);
        }
    }

    #endregion

    #region Shopping cart

    /// <summary>
    /// Restore the abandoned shopping cart
    /// </summary>
    /// <param name="cartId">Cart identifier</param>
    public async Task RestoreShoppingCartAsync(string cartId)
    {
        var res = await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{cartId}", httpMethod: HttpMethod.Get);

        if (string.IsNullOrEmpty(res))
            return;

        var restoredCart = JsonConvert.DeserializeObject<CartDto>(res)
            ?? throw new NopException("Cart data can't be loaded");

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        foreach (var cartProduct in restoredCart.Products)
        {
            var combination = await _productAttributeService.GetProductAttributeCombinationBySkuAsync(cartProduct.Sku);
            var product = combination == null ? await _productService.GetProductBySkuAsync(cartProduct.Sku) : await _productService.GetProductByIdAsync(combination.ProductId);

            if (product == null)
            {
                if (!int.TryParse(cartProduct.VariantId, out var variantId))
                    continue;

                product = await _productService.GetProductByIdAsync(variantId);

                if (product == null)
                    continue;
            }

            var shoppingCartItem = cart.FirstOrDefault(item => item.ProductId.ToString() == cartProduct.ProductId &&
                item.Quantity == cartProduct.Quantity &&
                item.AttributesXml == combination?.AttributesXml);
            if (shoppingCartItem is not null)
                continue;

            await _shoppingCartService.AddToCartAsync(customer, product, ShoppingCartType.ShoppingCart, store.Id,
                combination?.AttributesXml, quantity: cartProduct.Quantity);
        }
    }

    /// <summary>
    /// Adds new item to the shopping cart
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task AddShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, shoppingCartItem.StoreId);

        if (cart.Count == 1)
            await CreateCartAsync(cart);
        else
            await UpdateCartAsync(customer, shoppingCartItem);
    }

    /// <summary>
    /// Updates the shopping cart item
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task EditShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var item = await ShoppingCartItemToDtoAsync(shoppingCartItem, customer);

        var data = JsonConvert.SerializeObject(item);

        await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}/{item.CartProductId}", data, HttpMethod.Put);
    }

    /// <summary>
    /// Deletes item from shopping cart
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
        var customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, shoppingCartItem.StoreId);

        //var sendRequest = await _omnisendCustomerService.IsNeedToSendDeleteShoppingCartEventAsync(customer);

        //if (sendRequest)
        //    await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}/{shoppingCartItem.Id}", httpMethod: HttpMethod.Delete);

        if (!cart.Any())
        {
            //if (sendRequest)
            //    await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}", httpMethod: HttpMethod.Delete);

            await _omnisendCustomerService.DeleteCurrentCustomerShoppingCartIdAsync(customer);
        }
    }

    #endregion

    #region Orders

    /// <summary>
    /// Sends the new order to the omnisend service
    /// </summary>
    /// <param name="order">Order</param>
    public async Task PlaceOrderAsync(Order order)
    {
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        //await CreateOrderAsync(order);

        await _omnisendCustomerService.DeleteStoredCustomerShoppingCartIdAsync(customer);
    }

    /// <summary>
    /// Updates the order information
    /// </summary>
    /// <param name="order">Order</param>
    public async Task UpdateOrderAsync(Order order)
    {
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var item = await OrderToDtoAsync(order);
        if (item is null)
            return;

        var data = JsonConvert.SerializeObject(item);
        await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.OrdersApiUrl}/{item.OrderId}", data, HttpMethod.Put);
    }

    /// <summary>
    /// Store the CartId during order placing
    /// </summary>
    /// <param name="entity">Order item</param>
    /// <returns></returns>
    public async Task OrderItemAddedAsync(OrderItem entity)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        if (cart.Any(sci =>
                sci.ProductId == entity.ProductId &&
                sci.AttributesXml.Equals(entity.AttributesXml, StringComparison.InvariantCultureIgnoreCase) &&
                sci.Quantity == entity.Quantity))
            await _omnisendCustomerService.StoreCartIdAsync(customer);
    }

    #endregion


    #endregion
}