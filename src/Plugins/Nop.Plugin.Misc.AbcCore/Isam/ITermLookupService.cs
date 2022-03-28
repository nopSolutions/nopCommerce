using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface ITermLookupService
    {
        Task<(string termNo, string description, string link)> GetTermAsync(
            IList<ShoppingCartItem> cart
        );
    }
}