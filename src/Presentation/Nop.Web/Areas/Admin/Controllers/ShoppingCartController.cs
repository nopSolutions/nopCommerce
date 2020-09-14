using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ShoppingCartController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        #endregion

        #region Ctor

        public ShoppingCartController(ICustomerService customerService,
            IPermissionService permissionService,
            IShoppingCartService shoppingCartService,
            IShoppingCartModelFactory shoppingCartModelFactory)
        {
            _customerService = customerService;
            _permissionService = permissionService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
        }

        #endregion
        
        #region Methods
        
        public virtual async Task<IActionResult> CurrentCarts()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            //prepare model
            var model = await _shoppingCartModelFactory.PrepareShoppingCartSearchModel(new ShoppingCartSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CurrentCarts(ShoppingCartSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _shoppingCartModelFactory.PrepareShoppingCartListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCartDetails(ShoppingCartItemSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = await _customerService.GetCustomerById(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = await _shoppingCartModelFactory.PrepareShoppingCartItemListModel(searchModel, customer);

            return Json(model);
        }
        
        [HttpPost]
        public virtual async Task<IActionResult> DeleteItem(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedDataTablesJson();
            
            await _shoppingCartService.DeleteShoppingCartItem(id);

            return new NullJsonResult();
        }

        #endregion
    }
}