using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Services;

public interface ITireDealService
{
    Task<IEnumerable<TireDealModel>> GetAllAsync();
    Task<TireDealModel> GetByIdAsync(int id);
    Task InsertAsync(TireDealCreateModel model);
    Task UpdateAsync(TireDealUpdateModel model);
}