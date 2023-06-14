using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Mapping;

public class TireDealMapper : ITireDealMapper
{
    public TireDealModel ToModel(TireDealEntity entity)
    {
        return new TireDealModel()
        {
            Id = entity.Id,
            Title = entity.Title,
            LongDescription = entity.LongDescription,
            ShortDescription = entity.ShortDescription
        };
    }

    public IEnumerable<TireDealModel> ToModel(IEnumerable<TireDealEntity> entities)
    {
        return entities.Select(ToModel).ToList();
    }

    public TireDealEntity ToEntity(TireDealCreateModel model)
    {
        return new TireDealEntity()
        {
            Title = model.Title,
            LongDescription = model.LongDescription,
            ShortDescription = model.ShortDescription
        };
    }
}