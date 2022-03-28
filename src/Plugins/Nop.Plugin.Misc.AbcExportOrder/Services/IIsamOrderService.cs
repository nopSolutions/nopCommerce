using Nop.Core.Domain.Orders;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcExportOrder.Services
{
    public interface IIsamOrderService
    {
        Task InsertOrderAsync(Order order);
    }
}