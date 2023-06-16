using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
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

    public async Task<IList<TireDealModel>> GetAllAsync()
    {
        var models = _tireDealMapper.ToModel(await _dealRepository.GetAllAsync(deals => deals));

        return models;
    }

    public virtual async Task<IEnumerable<TireDealModel>> GetAllAsync(
        string title = null,
        string shortDescription = null,
        string longDescription = null,
        bool? isActive = true)
    {
        IEnumerable<TireDealEntity> deals;

        deals = await _dealRepository.GetAllAsync(query =>
        {
            if (isActive != null)
                query = query.Where(deal => deal.IsActive == true);

            if (title != null)
                query = query.Where(deal => deal.Title == title);

            if (shortDescription != null)
                query = query.Where(deal => deal.ShortDescription == shortDescription);

            if (longDescription != null)
                query = query.Where(deal => deal.LongDescription == longDescription);

            return query.AsQueryable();
        });
        
        return _tireDealMapper.ToModel(deals);
    }

    public async Task<TireDealModel> GetByIdAsync(int id)
    {
        return _tireDealMapper.ToModel(await _dealRepository.GetByIdAsync(id));
    }

    public async Task InsertAsync(TireDealCreateModel model)
    {
        await _dealRepository.InsertAsync(_tireDealMapper.ToEntity(model), false);
    }

    public async Task UpdateAsync(TireDealUpdateModel model)
    {
        var entity = await _dealRepository.GetByIdAsync(model.Id);
        entity.Title = model.Title;
        entity.IsActive = model.IsActive;
        entity.LongDescription = model.LongDescription;
        entity.ShortDescription = model.ShortDescription;
        
        await _dealRepository.UpdateAsync(entity);
    }
}