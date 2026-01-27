using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.Babak.Domain;

namespace Nop.Plugin.Misc.Babak.Services;

public class BabakItemService : IBabakItemService
{
    private readonly IRepository<BabakItem> _babakItemRepository;

    public BabakItemService(IRepository<BabakItem> babakItemRepository)
    {
        _babakItemRepository = babakItemRepository;
    }

    public async Task<IPagedList<BabakItem>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _babakItemRepository.Table.OrderBy(item => item.Id);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task<BabakItem> GetByIdAsync(int id)
    {
        return await _babakItemRepository.GetByIdAsync(id);
    }

    public async Task InsertAsync(BabakItem item)
    {
        await _babakItemRepository.InsertAsync(item);
    }

    public async Task UpdateAsync(BabakItem item)
    {
        await _babakItemRepository.UpdateAsync(item);
    }

    public async Task DeleteAsync(BabakItem item)
    {
        await _babakItemRepository.DeleteAsync(item);
    }
}
