using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Services;

public interface IDealService
{
    Task<IEnumerable<DealModel>> GetAllAsync();
    Task<DealModel> GetByIdAsync(int id);
    Task InsertAsync(DealModel model);
    Task UpdateAsync(DealUpdateModel model);
}