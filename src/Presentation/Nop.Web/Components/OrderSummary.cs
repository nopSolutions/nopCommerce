using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Components
{
    public class OrderSummaryViewComponent : ViewComponent
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public OrderSummaryViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            this._shoppingCartModelFactory = shoppingCartModelFactory;
            this._storeContext = storeContext;
            this._workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool? prepareAndDisplayOrderReviewData)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var model = new ShoppingCartModel();
            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart,
                isEditable: false,
                prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
            return View(model);
        }
    }
}
