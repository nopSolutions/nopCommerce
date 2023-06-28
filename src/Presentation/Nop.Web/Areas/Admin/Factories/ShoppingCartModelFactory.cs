using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the shopping cart model factory implementation
    /// </summary>
    public partial class ShoppingCartModelFactory : IShoppingCartModelFactory
    {
        #region Fields

        protected readonly CatalogSettings _catalogSettings;
        protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
        protected readonly ICountryService _countryService;
        protected readonly ICustomerService _customerService;
        protected readonly IDateTimeHelper _dateTimeHelper;
        protected readonly ILocalizationService _localizationService;
        protected readonly IPriceFormatter _priceFormatter;
        protected readonly IProductAttributeFormatter _productAttributeFormatter;
        protected readonly IProductService _productService;
        protected readonly IShoppingCartService _shoppingCartService;
        protected readonly IStoreService _storeService;
        protected readonly ITaxService _taxService;

        #endregion

        #region Ctor

        public ShoppingCartModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IStoreService storeService,
            ITaxService taxService)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _priceFormatter = priceFormatter;
            _productAttributeFormatter = productAttributeFormatter;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _storeService = storeService;
            _taxService = taxService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare shopping cart item search model
        /// </summary>
        /// <param name="searchModel">Shopping cart item search model</param>
        /// <returns>Shopping cart item search model</returns>
        protected virtual ShoppingCartItemSearchModel PrepareShoppingCartItemSearchModel(ShoppingCartItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare shopping cart search model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart search model
        /// </returns>
        public virtual async Task<ShoppingCartSearchModel> PrepareShoppingCartSearchModelAsync(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available shopping cart types
            await _baseAdminModelFactory.PrepareShoppingCartTypesAsync(searchModel.AvailableShoppingCartTypes, false);

            //set default search values
            searchModel.ShoppingCartType = ShoppingCartType.ShoppingCart;

            //prepare available billing countries
            searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
                .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
            searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare nested search model
            PrepareShoppingCartItemSearchModel(searchModel.ShoppingCartItemSearchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged shopping cart list model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart list model
        /// </returns>
        public virtual async Task<ShoppingCartListModel> PrepareShoppingCartListModelAsync(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customers with shopping carts
            var customers = await _customerService.GetCustomersWithShoppingCartsAsync(searchModel.ShoppingCartType,
                storeId: searchModel.StoreId,
                productId: searchModel.ProductId,
                createdFromUtc: searchModel.StartDate,
                createdToUtc: searchModel.EndDate,
                countryId: searchModel.BillingCountryId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new ShoppingCartListModel().PrepareToGridAsync(searchModel, customers, () =>
            {
                return customers.SelectAwait(async customer =>
                {
                    //fill in model values from the entity
                    var shoppingCartModel = new ShoppingCartModel
                    {
                        CustomerId = customer.Id
                    };

                    //fill in additional values (not existing in the entity)
                    shoppingCartModel.CustomerEmail = (await _customerService.IsRegisteredAsync(customer))
                        ? customer.Email
                        : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                    shoppingCartModel.TotalItems = (await _shoppingCartService
                        .GetShoppingCartAsync(customer, searchModel.ShoppingCartType,
                            searchModel.StoreId, searchModel.ProductId, searchModel.StartDate, searchModel.EndDate))
                        .Sum(item => item.Quantity);

                    return shoppingCartModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged shopping cart item list model
        /// </summary>
        /// <param name="searchModel">Shopping cart item search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart item list model
        /// </returns>
        public virtual async Task<ShoppingCartItemListModel> PrepareShoppingCartItemListModelAsync(ShoppingCartItemSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get shopping cart items
            var items = (await _shoppingCartService.GetShoppingCartAsync(customer, searchModel.ShoppingCartType,
                searchModel.StoreId, searchModel.ProductId, searchModel.StartDate, searchModel.EndDate)).ToPagedList(searchModel);

            var isSearchProduct = searchModel.ProductId > 0;

            Product product = null;

            if (isSearchProduct)
            {
                product = await _productService.GetProductByIdAsync(searchModel.ProductId) ?? throw new Exception("Product is not found");
            }

            var store = await _storeService.GetStoreByIdAsync(searchModel.StoreId);
            //prepare list model
            var model = await new ShoppingCartItemListModel().PrepareToGridAsync(searchModel, items, () =>
            {
                return items
                .OrderByDescending(item => item.CreatedOnUtc)
                .SelectAwait(async item =>
                {
                    //fill in model values from the entity
                    var itemModel = item.ToModel<ShoppingCartItemModel>();

                    if (!isSearchProduct)
                        product = await _productService.GetProductByIdAsync(item.ProductId);

                    //convert dates to the user time
                    itemModel.UpdatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(item.UpdatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    itemModel.Store = (await _storeService.GetStoreByIdAsync(item.StoreId))?.Name ?? "Deleted";
                    itemModel.AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, item.AttributesXml, customer, store);
                    var (unitPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(item, true);
                    itemModel.UnitPrice = await _priceFormatter.FormatPriceAsync((await _taxService.GetProductPriceAsync(product, unitPrice)).price);
                    itemModel.UnitPriceValue = (await _taxService.GetProductPriceAsync(product, unitPrice)).price;
                    var (subTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(item, true);
                    itemModel.Total = await _priceFormatter.FormatPriceAsync((await _taxService.GetProductPriceAsync(product, subTotal)).price);
                    itemModel.TotalValue = (await _taxService.GetProductPriceAsync(product, subTotal)).price;

                    //set product name since it does not survive mapping
                    itemModel.ProductName = product.Name;

                    return itemModel;
                });
            });

            return model;
        }

        #endregion
    }
}