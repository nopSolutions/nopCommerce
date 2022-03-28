using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.AbcExportOrder.Models;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcExportOrder.Services
{
    public interface IYahooService
    {
        Task<IList<YahooShipToRow>> GetYahooShipToRowsAsync(Order order);
        Task<IList<YahooHeaderRow>> GetYahooHeaderRowsAsync(Order order);
        Task<IList<YahooDetailRow>> GetYahooDetailRowsAsync(Order order);
    }
}