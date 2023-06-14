using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Mapping;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Services;

public class DealService : IDealService
{
    private readonly IRepository<DealEntity> _dealRepository;
    private readonly IDealMapper _dealMapper;

    public DealService(IRepository<DealEntity> dealRepository, IDealMapper dealMapper)
    {
        _dealRepository = dealRepository;
        _dealMapper = dealMapper;
    }

    public async Task<IEnumerable<DealModel>> GetAllAsync()
    {
        return _dealMapper.ToModel(_dealRepository.GetAll());
    }
    
    public async Task<DealModel> GetByIdAsync(int id)
    {
        return _dealMapper.ToModel(await _dealRepository.GetByIdAsync(id));
    }

    public Task InsertAsync(DealModel model)
    {
        return _dealRepository.InsertAsync(_dealMapper.ToEntity(model));
    }

    public Task UpdateAsync(DealUpdateModel model)
    {
        throw new System.NotImplementedException();
    }
}