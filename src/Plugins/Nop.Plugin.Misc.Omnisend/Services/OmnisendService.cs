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
public class OmnisendService(ICategoryService categoryService,
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
    #region Utilities

    private async Task<string> SendBatchAsync(string data)
    {
        var rez = await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

        if (rez == null)
            return string.Empty;

        var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

        if (bathId == null)
            return string.Empty;

        omnisendSettings.BatchesIds.Add(bathId);
        await settingService.SaveSettingAsync(omnisendSettings);

        return bathId;
    }

    private async Task<string> SendProductBatchAsync(IEnumerable<Product> products, HttpMethod batchMethod)
    {
        var data = JsonConvert.SerializeObject(new BatchRequest
        {
            Method = batchMethod.Method.ToUpper(),
            Endpoint = OmnisendDefaults.ProductsEndpoint,
            Items = await products.SelectAwait(async product => await ProductToDtoAsync(product) as IBatchSupport).ToListAsync()
        });

        return await SendBatchAsync(data);
    }

    /// <summary>
    /// Process the batch response
    /// </summary>
    /// <param name="batchResponse">Batch response to process</param>
    /// <returns>New batch identifier if it placed during process the current one</returns>
    public async Task<string> ProcessBatch(BatchResponse batchResponse)
    {
        var (delete, batchId) = await process();

        if (!delete)
            return batchId;

        omnisendSettings.BatchesIds.Remove(batchResponse.BatchId);
        await settingService.SaveSettingAsync(omnisendSettings);

        return batchId;

        async Task<(bool, string)> process()
        {
            var newBatchId = string.Empty;

            if (!batchResponse.Status.Equals(OmnisendDefaults.BatchFinishedStatus, StringComparison.InvariantCultureIgnoreCase))
            {
                batchResponse.ErrorsCount = 0;

                return (false, newBatchId);
            }

            var endpoint = batchResponse.Endpoint;

            if (!endpoint.Equals(OmnisendDefaults.ProductsEndpoint, StringComparison.InvariantCultureIgnoreCase) &&
                !endpoint.Equals(OmnisendDefaults.CategoriesEndpoint, StringComparison.InvariantCultureIgnoreCase))
                return (true, newBatchId);

            if (batchResponse.ErrorsCount == 0)
                return (true, newBatchId);

            if (batchResponse.Method.Equals("PUT", StringComparison.InvariantCultureIgnoreCase))
                return (false, newBatchId);

            var url = OmnisendDefaults.BatchesApiUrl + $"/{batchResponse.BatchId}/items";

            var result = await omnisendHttpClient.PerformRequestAsync<BatchItemsResponse>(url, httpMethod: HttpMethod.Get);

            var updateItems = new List<int>();

            foreach (var resultError in result.Errors)
            {
                if (resultError.ResponseCode != 400)
                    continue;

                var needChangeCount = false;

                if (endpoint.Equals(OmnisendDefaults.ProductsEndpoint, StringComparison.InvariantCultureIgnoreCase))
                {
                    var productDto = JsonConvert.DeserializeObject<ProductDto>(resultError.Request?.ToString() ?? string.Empty);

                    updateItems.Add(int.Parse(productDto.ProductId));
                    needChangeCount = true;
                }

                if (endpoint.Equals(OmnisendDefaults.CategoriesEndpoint, StringComparison.InvariantCultureIgnoreCase))
                {
                    var categoryDto = JsonConvert.DeserializeObject<CategoryDto>(resultError.Request?.ToString() ?? string.Empty);

                    updateItems.Add(int.Parse(categoryDto.CategoryId));
                    needChangeCount = true;
                }

                if (!needChangeCount)
                    continue;

                batchResponse.TotalCount--;
                batchResponse.ErrorsCount--;
            }

            if (!updateItems.Any())
                return (false, newBatchId);

            if (endpoint.Equals(OmnisendDefaults.ProductsEndpoint, StringComparison.InvariantCultureIgnoreCase))
                newBatchId = await UpdateProductsAsync(updateItems.ToArray());

            if (endpoint.Equals(OmnisendDefaults.CategoriesEndpoint, StringComparison.InvariantCultureIgnoreCase))
                await UpdateCategoriesAsync(updateItems.ToArray());

            return (batchResponse.ErrorsCount == 0, newBatchId);
        }
    }

    private async Task<List<BatchResponse>> GetBatchesInfoAsync(IList<string> batchesIds)
    {
        if (!batchesIds.Any())
            return [];

        var batches = await batchesIds.SelectAwait(GetBatchInfoAsync)
            .ToListAsync();

        batches = batches.Where(p => p != null).ToList();

        if (batches.Any())
            return batches;

        omnisendSettings.BatchesIds.Clear();
        await settingService.SaveSettingAsync(omnisendSettings);

        return batches;
    }

    private async Task FillCustomerInfoAsync(BaseContactInfoDto dto, Customer customer)
    {
        if (customer == null)
            return;

        dto.FirstName = customer.FirstName;
        dto.LastName = customer.LastName;

        var country = await countryService.GetCountryByIdAsync(customer.CountryId);

        if (country != null)
        {
            dto.Country = country.Name;
            dto.CountryCode = country.TwoLetterIsoCode;
        }

        var state = await stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId);

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
        var product = await productService.GetProductByIdAsync(orderItem.ProductId);

        var (sku, variantId) = await omnisendHelper.GetSkuAndVariantIdAsync(product, orderItem.AttributesXml);

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
            var productCategories = await categoryService.GetProductCategoriesByProductIdAsync(product.Id);

            return productCategories.Select(pc => pc.CategoryId.ToString()).ToList();
        }

        async Task<IList<ProductAttributeCombination>> getProductCombinations()
        {
            return await productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
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
                    stockQuantity = await productService.GetTotalStockQuantityAsync(product);

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
            Currency = await omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            ProductUrl = await omnisendHelper.GetProductUrlAsync(product),
            Images = new List<ProductDto.Image>
            {
                await omnisendHelper.GetProductPictureUrlAsync(product),
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
        var customer = await customerService.GetCustomerByIdAsync(order.CustomerId);
        var email = await omnisendCustomerService.GetEmailAsync(customer, order.BillingAddressId);
        if (string.IsNullOrEmpty(email))
            return null;

        var dto = new OrderDto
        {
            OrderId = order.OrderGuid.ToString(),
            Email = email,
            Currency = await omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            OrderSum = order.OrderTotal.ToCents(),
            SubTotalSum = order.OrderSubtotalInclTax.ToCents(),
            DiscountSum = order.OrderDiscount.ToCents(),
            TaxSum = order.OrderTax.ToCents(),
            ShippingSum = order.OrderShippingInclTax.ToCents(),
            CreatedAt = order.CreatedOnUtc.ToDtoString(),
            Products = await (await orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async oi => await OrderItemToDtoAsync(oi)).ToListAsync()
        };

        return dto;
    }

    private async Task<CartItemDto> ShoppingCartItemToDtoAsync(ShoppingCartItem shoppingCartItem, Customer customer)
    {
        var product = await productService.GetProductByIdAsync(shoppingCartItem.ProductId);

        var (sku, variantId) = await omnisendHelper.GetSkuAndVariantIdAsync(product, shoppingCartItem.AttributesXml);

        var (scSubTotal, _, _, _) = await shoppingCartService.GetSubTotalAsync(shoppingCartItem, true);

        var dto = new CartItemDto
        {
            CartProductId = shoppingCartItem.Id.ToString(),
            ProductId = shoppingCartItem.ProductId.ToString(),
            Sku = sku,
            VariantId = variantId.ToString(),
            Title = product.Name,
            Quantity = shoppingCartItem.Quantity,
            Currency = await omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            Price = (await taxService.GetProductPriceAsync(product, scSubTotal, true, customer)).price.ToCents()
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
        var customer = await customerService.GetCustomerByIdAsync(customerId);

        var items = await cart
            .SelectAwait(async ci => await ShoppingCartItemToDtoAsync(ci, customer))
            .ToListAsync();

        var cartSum = (await orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal
            ?.ToCents() ?? 0;

        var email = await omnisendCustomerService.GetEmailAsync(customer);
        if (string.IsNullOrEmpty(email))
            return null;

        var cartId = await omnisendCustomerService.GetCartIdAsync(customer);

        return new CartDto
        {
            Currency = await omnisendHelper.GetPrimaryStoreCurrencyCodeAsync(),
            Email = email,
            CartId = cartId,
            CartSum = cartSum,
            Products = items,
            CartRecoveryUrl = omnisendCustomerService.GetAbandonedCheckoutUrl(cartId)
        };
    }

    private async Task CreateOrderAsync(Order order)
    {
        var orderDto = await OrderToDtoAsync(order);
        if (orderDto is null)
            return;

        var data = JsonConvert.SerializeObject(orderDto);
        await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.OrdersApiUrl, data, HttpMethod.Post);
    }

    private async Task CreateCartAsync(IList<ShoppingCartItem> cart)
    {
        var cartDto = await GetCartDtoAsync(cart);
        if (cartDto is null)
            return;

        var data = JsonConvert.SerializeObject(cartDto);
        await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CartsApiUrl, data, HttpMethod.Post);
    }

    private async Task UpdateCartAsync(Customer customer, ShoppingCartItem newItem)
    {
        var item = await ShoppingCartItemToDtoAsync(newItem, customer);
        if (item is null)
            return;

        var data = JsonConvert.SerializeObject(item);
        await omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}", data, HttpMethod.Post);
    }

    private async Task<bool> CanSendRequestAsync(Customer customer)
    {
        return !string.IsNullOrEmpty(
            await omnisendCustomerService.GetEmailAsync(customer, customer.BillingAddressId));
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
        var subscriptions = (subscriber == null ? newsLetterSubscriptionRepository.Table : newsLetterSubscriptionRepository.Table.Where(nlsr => nlsr.Id.Equals(subscriber.Id)))
            .Where(subscription => subscription.StoreId == storeId)
            .OrderBy(subscription => subscription.Id)
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

        var contacts = from item in subscriptions
            join c in customerRepository.Table on item.Email equals c.Email
                into temp
            from c in temp.DefaultIfEmpty()
            where c == null || (c.Active && !c.Deleted)
            select new { subscription = item, customer = c };

        var contactsWithCountry = from item in contacts
            join cr in countryRepository.Table on item.customer.CountryId equals cr.Id
                into temp
            from cr in temp.DefaultIfEmpty()
            select new { item.customer, item.subscription, country = cr };

        var contactsWithState = from item in contactsWithCountry
            join sp in stateProvinceRepository.Table on item.customer.StateProvinceId equals sp.Id
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
        var store = await storeContext.GetCurrentStoreAsync();
        var su = await PrepareNewsletterSubscribersAsync(store.Id, 0, omnisendSettings.PageSize);

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

                var rez = await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

                var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

                if (bathId != null)
                {
                    omnisendSettings.BatchesIds.Add(bathId);
                    await settingService.SaveSettingAsync(omnisendSettings);
                }

                page++;

                su = await PrepareNewsletterSubscribersAsync(store.Id, page, omnisendSettings.PageSize);

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
        var categories = await categoryService.GetAllCategoriesAsync(null, pageSize: omnisendSettings.PageSize);

        if (categories.TotalCount >= OmnisendDefaults.MinCountToUseBatch || categories.TotalCount > omnisendSettings.PageSize)
        {
            var page = 0;

            while (page < categories.TotalPages)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
                    Endpoint = OmnisendDefaults.CategoriesEndpoint,
                    Items = categories.Select(category => CategoryToDto(category) as IBatchSupport).ToList()
                });

                await SendBatchAsync(data);

                page++;

                categories = await categoryService.GetAllCategoriesAsync(null, pageIndex: page, pageSize: omnisendSettings.PageSize);
            }
        }
        else
            foreach (var category in categories)
            {
                var data = JsonConvert.SerializeObject(CategoryToDto(category));
                await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CategoriesApiUrl, data, HttpMethod.Post);
            }
    }

    /// <summary>
    /// Synchronize categories
    /// </summary>
    /// <param name="categoriesId">Categories identifiers list to update</param>
    public async Task UpdateCategoriesAsync(int[] categoriesId)
    {
        var categories = await categoryService.GetCategoriesByIdsAsync(categoriesId);

        foreach (var category in categories)
        {
            var data = JsonConvert.SerializeObject(CategoryToDto(category));
            await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CategoriesApiUrl + $"/{category.Id}", data, HttpMethod.Put);
        }
    }

    /// <summary>
    /// Synchronize products
    /// </summary>
    public async Task SyncProductsAsync()
    {
        var products = await productService.SearchProductsAsync(pageSize: omnisendSettings.PageSize);

        if (products.TotalCount >= OmnisendDefaults.MinCountToUseBatch || products.TotalCount > omnisendSettings.PageSize)
        {
            var page = 0;

            while (page < products.TotalPages)
            {
                await SendProductBatchAsync(products, HttpMethod.Post);

                page++;

                products = await productService.SearchProductsAsync(pageIndex: page, pageSize: omnisendSettings.PageSize);
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
        var orders = await orderService.SearchOrdersAsync(pageSize: omnisendSettings.PageSize);

        if (orders.TotalCount >= OmnisendDefaults.MinCountToUseBatch || orders.TotalCount > omnisendSettings.PageSize)
        {
            var page = 0;

            while (page < orders.TotalPages)
            {
                var data = JsonConvert.SerializeObject(new BatchRequest
                {
                    Endpoint = OmnisendDefaults.OrdersEndpoint,
                    Items = await orders.SelectAwait(async order => await OrderToDtoAsync(order) as IBatchSupport).ToListAsync()
                });

                var rez = await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

                var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

                if (bathId != null)
                {
                    omnisendSettings.BatchesIds.Add(bathId);
                    await settingService.SaveSettingAsync(omnisendSettings);
                }

                page++;

                orders = await orderService.SearchOrdersAsync(pageIndex: page, pageSize: omnisendSettings.PageSize);
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
        var store = await storeContext.GetCurrentStoreAsync();
        var customers = await customerService.GetCustomersWithShoppingCartsAsync(ShoppingCartType.ShoppingCart, store.Id);
        foreach (var customer in customers)
        {
            var cart = await shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            await CreateCartAsync(cart);
        }
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Gets the stored batches
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the stored batches
    /// </returns>
    public async Task<IList<BatchResponse>> GetStoredBatchesAsync()
    {
        var batches = await GetBatchesInfoAsync(omnisendSettings.BatchesIds);

        var additionalBatches = await batches
            .Where(p => p.Status.Equals(OmnisendDefaults.BatchFinishedStatus,
                StringComparison.InvariantCultureIgnoreCase))
            .SelectAwait(async batchResponse => await ProcessBatch(batchResponse))
            .Where(newBatchId => !string.IsNullOrEmpty(newBatchId)).ToListAsync();
        
        batches.AddRange(await GetBatchesInfoAsync(additionalBatches));

        return batches.Where(b => b.TotalCount > 0).ToList();
    }

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
        omnisendHttpClient.ApiKey = apiKey;

        var result = await omnisendHttpClient.PerformRequestAsync<AccountsResponse>(OmnisendDefaults.AccountsApiUrl);

        return result?.BrandId;
    }

    /// <summary>
    /// Registers the site on the omnisend service
    /// </summary>
    public async Task SendCustomerDataAsync()
    {
        var site = webHelper.GetStoreLocation();

        var data = JsonConvert.SerializeObject(new
        {
            website = site,
            platform = OmnisendDefaults.IntegrationOrigin,
            version = OmnisendDefaults.IntegrationVersion
        });

        await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.AccountsApiUrl, data, HttpMethod.Post);
    }

    /// <summary>
    /// Gets information about batch
    /// </summary>
    /// <param name="bathId">Batch identifier</param>
    public async ValueTask<BatchResponse> GetBatchInfoAsync(string bathId)
    {
        var url = OmnisendDefaults.BatchesApiUrl + $"/{bathId}";

        var result = await omnisendHttpClient.PerformRequestAsync<BatchResponse>(url, httpMethod: HttpMethod.Get);

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

        var res = await omnisendHttpClient.PerformRequestAsync<ContactInfoDto>(url, httpMethod: HttpMethod.Get);

        return res;
    }

    /// <summary>
    /// Update or create contact information
    /// </summary>
    /// <param name="request">Create contact request</param>
    public async Task UpdateOrCreateContactAsync(CreateContactRequest request)
    {
        var email = request.Identifiers.First().Id;
        var exists = !string.IsNullOrEmpty(await omnisendCustomerService.GetContactIdAsync(email));

        if (!exists)
        {
            var data = JsonConvert.SerializeObject(request);
            await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.ContactsApiUrl, data, HttpMethod.Post);
        }
        else
        {
            var url = $"{OmnisendDefaults.ContactsApiUrl}?email={email}";
            var data = JsonConvert.SerializeObject(new { identifiers = new[] { request.Identifiers.First() } });

            await omnisendHttpClient.PerformRequestAsync(url, data, HttpMethod.Patch);
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
            0, omnisendSettings.PageSize,
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
        var contactId = await omnisendCustomerService.GetContactIdAsync(customer.Email);

        if (string.IsNullOrEmpty(contactId))
            return;

        var url = $"{OmnisendDefaults.ContactsApiUrl}/{contactId}";

        var dto = new BaseContactInfoDto();

        await FillCustomerInfoAsync(dto, customer);

        var data = JsonConvert.SerializeObject(dto);

        await omnisendHttpClient.PerformRequestAsync(url, data, HttpMethod.Patch);
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
        await omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.ProductsApiUrl, data, HttpMethod.Post);
    }

    /// <summary>
    /// Updates the product
    /// </summary>
    /// <param name="productId">Product identifier to update</param>
    public async Task UpdateProductAsync(int productId)
    {
        var product = await productService.GetProductByIdAsync(productId);

        await CreateOrUpdateProductAsync(product);
    }

    /// <summary>
    /// Updates products
    /// </summary>
    /// /// <param name="productsId">Products identifiers list to update</param>
    public async Task<string> UpdateProductsAsync(int[] productsId)
    {
        var products = await productService.GetProductsByIdsAsync(productsId);

        return await SendProductBatchAsync(products, HttpMethod.Put);
    }

    /// <summary>
    /// Updates the product
    /// </summary>
    /// <param name="product">Product to update</param>
    public async Task CreateOrUpdateProductAsync(Product product)
    {
        var result = await omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.ProductsApiUrl}/{product.Id}", httpMethod: HttpMethod.Get);
        if (string.IsNullOrEmpty(result))
            await AddNewProductAsync(product);
        else
        {
            var data = JsonConvert.SerializeObject(await ProductToDtoAsync(product));
            await omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.ProductsApiUrl}/{product.Id}", data, HttpMethod.Put);
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
        var res = await omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{cartId}", httpMethod: HttpMethod.Get);

        if (string.IsNullOrEmpty(res))
            return;

        var restoredCart = JsonConvert.DeserializeObject<CartDto>(res)
            ?? throw new NopException("Cart data can't be loaded");

        var customer = await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();
        var cart = await shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        foreach (var cartProduct in restoredCart.Products)
        {
            var combination = await productAttributeService.GetProductAttributeCombinationBySkuAsync(cartProduct.Sku);
            var product = combination == null ? await productService.GetProductBySkuAsync(cartProduct.Sku) : await productService.GetProductByIdAsync(combination.ProductId);

            if (product == null)
            {
                if (!int.TryParse(cartProduct.VariantId, out var variantId))
                    continue;

                product = await productService.GetProductByIdAsync(variantId);

                if (product == null)
                    continue;
            }

            var shoppingCartItem = cart.FirstOrDefault(item => item.ProductId.ToString() == cartProduct.ProductId &&
                item.Quantity == cartProduct.Quantity &&
                item.AttributesXml == combination?.AttributesXml);
            if (shoppingCartItem is not null)
                continue;

            await shoppingCartService.AddToCartAsync(customer, product, ShoppingCartType.ShoppingCart, store.Id,
                combination?.AttributesXml, quantity: cartProduct.Quantity);
        }
    }

    /// <summary>
    /// Adds new item to the shopping cart
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task AddShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
        var customer = await customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var cart = await shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, shoppingCartItem.StoreId);

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
        var customer = await customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var item = await ShoppingCartItemToDtoAsync(shoppingCartItem, customer);

        var data = JsonConvert.SerializeObject(item);

        await omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}/{item.CartProductId}", data, HttpMethod.Put);
    }

    /// <summary>
    /// Deletes item from shopping cart
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public async Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem)
    {
        var customer = await customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var cart = await shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, shoppingCartItem.StoreId);

        //var sendRequest = await _omnisendCustomerService.IsNeedToSendDeleteShoppingCartEventAsync(customer);

        //if (sendRequest)
        //    await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}/{OmnisendDefaults.ProductsEndpoint}/{shoppingCartItem.Id}", httpMethod: HttpMethod.Delete);

        if (!cart.Any())
        {
            //if (sendRequest)
            //    await _omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.CartsApiUrl}/{await _omnisendCustomerService.GetCartIdAsync(customer)}", httpMethod: HttpMethod.Delete);

            await omnisendCustomerService.DeleteCurrentCustomerShoppingCartIdAsync(customer);
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
        var customer = await customerService.GetCustomerByIdAsync(order.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        //await CreateOrderAsync(order);

        await omnisendCustomerService.DeleteStoredCustomerShoppingCartIdAsync(customer);
    }

    /// <summary>
    /// Updates the order information
    /// </summary>
    /// <param name="order">Order</param>
    public async Task UpdateOrderAsync(Order order)
    {
        var customer = await customerService.GetCustomerByIdAsync(order.CustomerId);

        if (!await CanSendRequestAsync(customer))
            return;

        var item = await OrderToDtoAsync(order);
        if (item is null)
            return;

        var data = JsonConvert.SerializeObject(item);
        await omnisendHttpClient.PerformRequestAsync($"{OmnisendDefaults.OrdersApiUrl}/{item.OrderId}", data, HttpMethod.Put);
    }

    /// <summary>
    /// Store the CartId during order placing
    /// </summary>
    /// <param name="entity">Order item</param>
    /// <returns></returns>
    public async Task OrderItemAddedAsync(OrderItem entity)
    {
        var customer = await workContext.GetCurrentCustomerAsync();
        var store = await storeContext.GetCurrentStoreAsync();
        var cart = await shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        if (cart.Any(sci =>
                sci.ProductId == entity.ProductId &&
                sci.AttributesXml.Equals(entity.AttributesXml, StringComparison.InvariantCultureIgnoreCase) &&
                sci.Quantity == entity.Quantity))
            await omnisendCustomerService.StoreCartIdAsync(customer);
    }

    #endregion

    #endregion

    #region Properties

    /// <summary>
    /// Check whether the plugin is configured
    /// </summary>
    /// <returns>Result</returns>
    public bool IsConfigured => !string.IsNullOrEmpty(omnisendSettings.ApiKey);

    #endregion
}