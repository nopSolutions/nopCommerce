using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Services;

public interface ITireDealService
{
    Task<IList<TireDealModel>> GetAllAsync();
    Task<IEnumerable<TireDealModel>> GetAllAsync(
        string title = null,
        string shortDescription = null,
        string longDescription = null,
        bool? isActive = true);
    Task<TireDealModel> GetByIdAsync(int id);
    Task InsertAsync(TireDealCreateModel model);
    Task UpdateAsync(TireDealUpdateModel model);
}