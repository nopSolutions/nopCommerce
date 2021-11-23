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

        protected ICustomerService CustomerService { get; }
        protected IPermissionService PermissionService { get; }
        protected IShoppingCartModelFactory ShoppingCartModelFactory { get; }
        protected IShoppingCartService ShoppingCartService { get; }

        #endregion

        #region Ctor

        public ShoppingCartController(ICustomerService customerService,
            IPermissionService permissionService,
            IShoppingCartService shoppingCartService,
            IShoppingCartModelFactory shoppingCartModelFactory)
        {
            CustomerService = customerService;
            PermissionService = permissionService;
            ShoppingCartModelFactory = shoppingCartModelFactory;
            ShoppingCartService = shoppingCartService;
        }

        #endregion
        
        #region Methods
        
        public virtual async Task<IActionResult> CurrentCarts()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            //prepare model
            var model = await ShoppingCartModelFactory.PrepareShoppingCartSearchModelAsync(new ShoppingCartSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CurrentCarts(ShoppingCartSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ShoppingCartModelFactory.PrepareShoppingCartListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCartDetails(ShoppingCartItemSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
                return await AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = await ShoppingCartModelFactory.PrepareShoppingCartItemListModelAsync(searchModel, customer);

            return Json(model);
        }
        
        [HttpPost]
        public virtual async Task<IActionResult> DeleteItem(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
                return await AccessDeniedDataTablesJson();
            
            await ShoppingCartService.DeleteShoppingCartItemAsync(id);

            return new NullJsonResult();
        }

        #endregion
    }
}