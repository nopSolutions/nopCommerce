using Nop.Core;
using Nop.Plugin.Misc.Babak.Domain;

namespace Nop.Plugin.Misc.Babak.Services;

public interface IBabakItemService
{
    Task<IPagedList<BabakItem>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue);

    Task<BabakItem> GetByIdAsync(int id);

    Task InsertAsync(BabakItem item);

    Task UpdateAsync(BabakItem item);

    Task DeleteAsync(BabakItem item);
}
