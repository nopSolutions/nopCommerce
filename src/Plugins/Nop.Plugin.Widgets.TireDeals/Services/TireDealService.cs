using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Mapping;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Services;

public class TireDealService : ITireDealService
{
    private readonly IRepository<TireDealEntity> _dealRepository;
    private readonly ITireDealMapper _tireDealMapper;

    public TireDealService(IRepository<TireDealEntity> dealRepository, ITireDealMapper tireDealMapper)
    {
        _dealRepository = dealRepository;
        _tireDealMapper = tireDealMapper;
    }
 
    public async Task<IEnumerable<TireDealModel>> GetAllAsync()
    {
        return _tireDealMapper.ToModel(_dealRepository.GetAll());
    }
    
    public async Task<TireDealModel> GetByIdAsync(int id)
    {
        return _tireDealMapper.ToModel(await _dealRepository.GetByIdAsync(id));
    }

    public async Task InsertAsync(TireDealCreateModel model)
    {
        await _dealRepository.InsertAsync(_tireDealMapper.ToEntity(model), false);
    }

    public Task UpdateAsync(TireDealUpdateModel model)
    {
        throw new System.NotImplementedException();
    }
}