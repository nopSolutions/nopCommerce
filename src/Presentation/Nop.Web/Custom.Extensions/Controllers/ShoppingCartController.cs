using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Orders;
using Nop.Services.Security;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Threading.Tasks;

namespace Nop.Web.Controllers
{
    public partial class ShoppingCartController
    {

        [HttpPost]
        public virtual async Task<IActionResult> SendInterestAsync(int productId, IFormCollection form = null)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            //Create a shopping cart item as 'Interest'

            var pm = new PrivateMessage
            {
                StoreId = (_storeContext.GetCurrentStoreAsync()).Id,
                ToCustomerId = product.VendorId, //Vendor id in Product table is customer id
                FromCustomerId = (_workContext.GetCurrentCustomerAsync()).Id,
                Subject = "Interest Received",
                Text = "You have received an interest.Please get in touch and discuss further",
                IsDeletedByAuthor = false,
                IsDeletedByRecipient = false,
                IsRead = false,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _forumService.InsertPrivateMessageAsync(pm);

            return Json(new
            {
                success = true,
                message = "Your interest has been sent successfully.",
                messagetype = "InterestSent",
                productId = product.VendorId
            });
        }

        [HttpsRequirement]
        public virtual async Task<IActionResult> ShortListedAsync(Guid? customerGuid)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("Homepage");

            var customer = customerGuid.HasValue ? await _customerService.GetCustomerByGuidAsync(customerGuid.Value) : await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return RedirectToRoute("Homepage");

            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, (_storeContext.GetCurrentStoreAsync()).Id);

            var model = new WishlistModel();
            model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, cart, !customerGuid.HasValue);
            return View(model);
        }


    }
}
