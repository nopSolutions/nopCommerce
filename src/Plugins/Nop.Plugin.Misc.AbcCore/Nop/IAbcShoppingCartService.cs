using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public interface IAbcShoppingCartService : IShoppingCartService
    {
        Task<bool> IsCartEligibleForCheckoutAsync(object model);
    }
}
