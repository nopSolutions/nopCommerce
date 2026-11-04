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
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Orders;

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Represents the main plugin service class
/// </summary>
public class OmnisendService
{
    #region Fields

    private readonly ICategoryService _categoryService;
    private readonly ICountryService _countryService;
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
        IWebHelper webHelper,
        IWorkContext workContext,
        OmnisendCustomerService omnisendCustomerService,
        OmnisendHelper omnisendHelper,
        OmnisendHttpClient omnisendHttpClient,
        OmnisendSettings omnisendSettings)
    {
        _categoryService = categoryService;
        _countryService = countryService;
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
        _webHelper = webHelper;
        _workContext = workContext;
        _omnisendCustomerService = omnisendCustomerService;
        _omnisendHelper = omnisendHelper;
        _omnisendHttpClient = omnisendHttpClient;
        _omnisendSettings = omnisendSettings;
    }

    #endregion

    #region Utilities

    private async Task<string> SendBatchAsync(string data)
    {
        var rez = await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.BatchesApiUrl, data, HttpMethod.Post);

        if (rez == null)
            return string.Empty;

        var bathId = JsonConvert.DeserializeAnonymousType(rez, new { batchID = "" })?.batchID;

        if (bathId == null)
            return string.Empty;

        _omnisendSettings.BatchesIds.Add(bathId);
        await _settingService.SaveSettingAsync(_omnisendSettings);

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
    private async Task<string> ProcessBatchAsync(BatchResponse batchResponse)
    {
        var (delete, batchId) = await process();

        if (!delete)
            return batchId;

        _omnisendSettings.BatchesIds.Remove(batchResponse.BatchId);
        await _settingService.SaveSettingAsync(_omnisendSettings);

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

            var result = await _omnisendHttpClient.PerformRequestAsync<BatchItemsResponse>(url, httpMethod: HttpMethod.Get);

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

        _omnisendSettings.BatchesIds.Clear();
        await _settingService.SaveSettingAsync(_omnisendSettings);

        return batches;
    }

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
            Images = new List<string>
            {
                await _omnisendHelper.GetProductPictureUrlAsync(product),
            },
            CreatedAt = product.CreatedOnUtc.ToDtoString(),
            UpdatedAt = product.UpdatedOnUtc.ToDtoString(),
            CategoryIDs = await getProductCategories(),
            Variants =
            [
                new ProductDto.Variant
                {
                    VariantId = product.Id.ToString(),
                    Title = product.Name,
                    Sku = product.Sku,
                    Status = await getProductStatus(),
                    Price = (float)Math.Round(product.Price, 2),
                    ProductUrl = await _omnisendHelper.GetProductUrlAsync(product)
                }
            ]
        };

        if (combinations.Any())
        {
            dto.Variants.AddRange(await combinations.SelectAwait(async c => new ProductDto.Variant
            {
                VariantId = c.Id.ToString(),
                Title = product.Name,
                Sku = c.Sku,
                Status = await getProductStatus(c),
                Price = (float)Math.Round(c.OverriddenPrice ?? product.Price, 2),
                ProductUrl = await _omnisendHelper.GetProductUrlAsync(product)
            }).ToListAsync());
        }

        return dto;
    }
    
    private CategoryDto CategoryToDto(Category category)
    {
        return new CategoryDto
        {
            CategoryId = category.Id.ToString(),
            Title = category.Name
        };
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
            .Select(subscription => new { subscription.Email, subscription.Active, subscription.CreatedOnUtc })
            .Distinct()
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

        var contacts =
            from item in subscriptions
            join c in _customerRepository.Table on item.Email equals c.Email
                into temp
            from c in temp.DefaultIfEmpty()
            where c == null || (c.Active && !c.Deleted)
            select new { subscription = item, customer = c };

        var contactsWithCountry =
            from item in contacts
            join cr in _countryRepository.Table on item.customer.CountryId equals cr.Id
                into temp
            from cr in temp.DefaultIfEmpty()
            select new { item.customer, item.subscription, country = cr };

        var contactsWithState =
            from item in contactsWithCountry
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
            var dto = new CreateContactRequest(item.subscription.Email, item.subscription.Active, item.subscription.CreatedOnUtc, inactiveStatus, sendWelcomeMessage)
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
        {
            foreach (var newsLetterSubscription in su)
                await UpdateOrCreateContactAsync(newsLetterSubscription as CreateContactRequest);
        }
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
                    Endpoint = OmnisendDefaults.CategoriesBatchEndpoint,
                    Items = categories.Select(category => CategoryToDto(category) as IBatchSupport).ToList()
                });

                await SendBatchAsync(data);

                page++;

                categories = await _categoryService.GetAllCategoriesAsync(null, pageIndex: page, pageSize: _omnisendSettings.PageSize);
            }
        }
        else
        {
            foreach (var category in categories)
            {
                var data = JsonConvert.SerializeObject(CategoryToDto(category));
                await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CategoriesApiUrl, data, HttpMethod.Post);
            }
        }
    }

    /// <summary>
    /// Synchronize categories
    /// </summary>
    /// <param name="categoriesId">Categories identifiers list to update</param>
    public async Task UpdateCategoriesAsync(int[] categoriesId)
    {
        var categories = await _categoryService.GetCategoriesByIdsAsync(categoriesId);

        foreach (var category in categories)
        {
            var data = JsonConvert.SerializeObject(CategoryToDto(category));
            await _omnisendHttpClient.PerformRequestAsync(OmnisendDefaults.CategoriesApiUrl + $"/{category.Id}", data, HttpMethod.Patch);
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
                await SendProductBatchAsync(products, HttpMethod.Post);

                page++;

                products = await _productService.SearchProductsAsync(pageIndex: page, pageSize: _omnisendSettings.PageSize);
            }
        }
        else
        {
            foreach (var product in products)
                await AddNewProductAsync(product);
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
        var batches = await GetBatchesInfoAsync(_omnisendSettings.BatchesIds);

        var additionalBatches = await batches
            .Where(p => p.Status.Equals(OmnisendDefaults.BatchFinishedStatus,
                StringComparison.InvariantCultureIgnoreCase))
            .SelectAwait(async batchResponse => await ProcessBatchAsync(batchResponse))
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
    /// Updates products
    /// </summary>
    /// /// <param name="productsId">Products identifiers list to update</param>
    public async Task<string> UpdateProductsAsync(int[] productsId)
    {
        var products = await _productService.GetProductsByIdsAsync(productsId);

        return await SendProductBatchAsync(products, HttpMethod.Put);
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
    
    #region Orders
    
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
        {
            await _omnisendCustomerService.StoreCartIdAsync(customer);
        }
    }

    #endregion

    #endregion

    #region Properties

    /// <summary>
    /// Check whether the plugin is configured
    /// </summary>
    /// <returns>Result</returns>
    public bool IsConfigured => !string.IsNullOrEmpty(_omnisendSettings.ApiKey);

    #endregion
}