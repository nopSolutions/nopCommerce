using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Widget.Deals.Domain;
using Nop.Plugin.Widget.Deals.Models;

namespace Nop.Plugin.Widget.Deals.Services;

public class DealService : IDealService
{
    private IRepository<Deal> _dealRepository;

    public DealService(IRepository<Deal> dealRepository)
    {
        _dealRepository = dealRepository;
    }

    public async Task<IEnumerable<Deal>> GetAll()
    {
        return _dealRepository.GetAll();
    }

    public Deal GetById(int id)
    {
        throw new System.NotImplementedException();
    }

    public async Task Insert(Deal entity)
    {
        await _dealRepository.InsertAsync(entity);
    }

    public void Update(Deal deal)
    {
        throw new System.NotImplementedException();
    }
}