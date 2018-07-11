using System;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the shopping cart model factory implementation
    /// </summary>
    public partial class ShoppingCartModelFactory : IShoppingCartModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;

        #endregion

        #region Ctor

        public ShoppingCartModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IStoreService storeService,
            ITaxService taxService)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._productAttributeFormatter = productAttributeFormatter;
            this._storeService = storeService;
            this._taxService = taxService;
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
        /// <returns>Shopping cart search model</returns>
        public virtual ShoppingCartSearchModel PrepareShoppingCartSearchModel(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available shopping cart types
            _baseAdminModelFactory.PrepareShoppingCartTypes(searchModel.AvailableShoppingCartTypes, false);

            //set default search values
            searchModel.ShoppingCartType = ShoppingCartType.ShoppingCart;

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
        /// <returns>Shopping cart list model</returns>
        public virtual ShoppingCartListModel PrepareShoppingCartListModel(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customers with shopping carts
            var customers = _customerService.GetAllCustomers(loadOnlyWithShoppingCart: true,
                sct: searchModel.ShoppingCartType,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new ShoppingCartListModel
            {
                Data = customers.Select(customer =>
                {
                    //fill in model values from the entity
                    var shoppingCartModel = new ShoppingCartModel
                    {
                        CustomerId = customer.Id
                    };

                    //fill in additional values (not existing in the entity)
                    shoppingCartModel.CustomerEmail = customer.IsRegistered()
                        ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    shoppingCartModel.TotalItems = customer.ShoppingCartItems
                        .Where(item => item.ShoppingCartType == searchModel.ShoppingCartType).Sum(item => item.Quantity);

                    return shoppingCartModel;
                }),
                Total = customers.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare paged shopping cart item list model
        /// </summary>
        /// <param name="searchModel">Shopping cart item search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shopping cart item list model</returns>
        public virtual ShoppingCartItemListModel PrepareShoppingCartItemListModel(ShoppingCartItemSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get shopping cart items
            var items = customer.ShoppingCartItems.Where(item => item.ShoppingCartType == searchModel.ShoppingCartType).ToList();

            //prepare list model
            var model = new ShoppingCartItemListModel
            {
                Data = items.PaginationByRequestModel(searchModel).Select(item =>
                {
                    //fill in model values from the entity
                    var itemModel = new ShoppingCartItemModel
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = item.Product?.Name
                    };

                    //convert dates to the user time
                    itemModel.UpdatedOn = _dateTimeHelper.ConvertToUserTime(item.UpdatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    itemModel.Store = _storeService.GetStoreById(item.StoreId)?.Name ?? "Deleted";
                    itemModel.AttributeInfo = _productAttributeFormatter.FormatAttributes(item.Product, item.AttributesXml, item.Customer);
                    var unitPrice = _priceCalculationService.GetUnitPrice(item);
                    itemModel.UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(item.Product, unitPrice, out var _));
                    var subTotal = _priceCalculationService.GetSubTotal(item);
                    itemModel.Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(item.Product, subTotal, out _));

                    return itemModel;
                }),
                Total = items.Count
            };

            return model;
        }

        #endregion
    }
}