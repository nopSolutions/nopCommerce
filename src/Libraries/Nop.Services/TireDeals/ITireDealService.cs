using System.Collections;
using Nop.Core.Domain.TireDeals;

namespace Nop.Services.TireDeals;

public interface ITireDealService
{
    Task<IList<TireDeal>> GetAllAsync();
    Task<IList<TireDeal>> GetAllActiveAsync();
    Task<IList<TireDeal>> GetAllAsync(string? title = null);
    Task<TireDeal> GetByIdAsync(int id);
    Task InsertAsync(TireDeal model);
    Task UpdateAsync(TireDeal model);
}