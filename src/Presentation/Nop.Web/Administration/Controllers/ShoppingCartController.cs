using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.ShoppingCart;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class ShoppingCartController : BaseNopController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Constructors

        public ShoppingCartController(ICustomerService customerService,
            IDateTimeHelper dateTimeHelper, IPriceFormatter priceFormatter,
            IStoreService storeService, ITaxService taxService, 
            IPriceCalculationService priceCalculationService,
            IPermissionService permissionService, ILocalizationService localizationService)
        {
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._storeService = storeService;
            this._taxService = taxService;
            this._priceCalculationService = priceCalculationService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
        }

        #endregion
        
        #region Methods

        //shopping carts
        public ActionResult CurrentCarts()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CurrentCarts(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customers = _customerService.GetAllCustomers(
                loadOnlyWithShoppingCart: true,
                sct: ShoppingCartType.ShoppingCart,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new GridModel<ShoppingCartModel>
            {
                Data = customers.Select(x =>
                {
                    return new ShoppingCartModel()
                    {
                        CustomerId = x.Id,
                        CustomerEmail = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                        TotalItems = x.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList().GetTotalProducts()
                    };
                }),
                Total = customers.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetCartDetails(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            var gridModel = new GridModel<ShoppingCartItemModel>()
            {
                Data = cart.Select(sci =>
                {
                    decimal taxRate;
                    var store = _storeService.GetStoreById(sci.StoreId); 
                    var sciModel = new ShoppingCartItemModel()
                    {
                        Id = sci.Id,
                        Store = store != null ? store.Name : "Unknown",
                        ProductVariantId = sci.ProductVariantId,
                        Quantity = sci.Quantity,
                        FullProductName = sci.ProductVariant.FullProductName,
                        UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetUnitPrice(sci, true), out taxRate)),
                        Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetSubTotal(sci, true), out taxRate)),
                        UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.UpdatedOnUtc, DateTimeKind.Utc)
                    };
                    return sciModel;
                }),
                Total = cart.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }





        //wishlists
        public ActionResult CurrentWishlists()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CurrentWishlists(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customers = _customerService.GetAllCustomers(
                loadOnlyWithShoppingCart: true,
                sct: ShoppingCartType.Wishlist,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new GridModel<ShoppingCartModel>
            {
                Data = customers.Select(x =>
                {
                    return new ShoppingCartModel()
                    {
                        CustomerId = x.Id,
                        CustomerEmail = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                        TotalItems = x.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList().GetTotalProducts()
                    };
                }),
                Total = customers.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetWishlistDetails(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            var cart = customer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            var gridModel = new GridModel<ShoppingCartItemModel>()
            {
                Data = cart.Select(sci =>
                {
                    decimal taxRate;
                    var store = _storeService.GetStoreById(sci.StoreId); 
                    var sciModel = new ShoppingCartItemModel()
                    {
                        Id = sci.Id,
                        Store = store != null ? store.Name : "Unknown",
                        ProductVariantId = sci.ProductVariantId,
                        Quantity = sci.Quantity,
                        FullProductName = sci.ProductVariant.FullProductName,
                        UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetUnitPrice(sci, true), out taxRate)),
                        Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetSubTotal(sci, true), out taxRate)),
                        UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.UpdatedOnUtc, DateTimeKind.Utc)
                    };
                    return sciModel;
                }),
                Total = cart.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion
    }
}
