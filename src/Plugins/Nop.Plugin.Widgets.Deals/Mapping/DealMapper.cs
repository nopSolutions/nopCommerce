using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Mapping;

public class DealMapper : IDealMapper
{
    public DealModel ToModel(DealEntity entity)
    {
        return new DealModel()
        {
            Id = entity.Id,
            Title = entity.Title,
            LongDescription = entity.LongDescription,
            ShortDescription = entity.ShortDescription
        };
    }

    public IEnumerable<DealModel> ToModel(IEnumerable<DealEntity> entities)
    {
        return entities.Select(ToModel).ToList();
    }

    public DealEntity ToEntity(DealModel model)
    {
        return new DealEntity()
        {
            Title = model.Title,
            LongDescription = model.LongDescription,
            ShortDescription = model.ShortDescription
        };
    }
}