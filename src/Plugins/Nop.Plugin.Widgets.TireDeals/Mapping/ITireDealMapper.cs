using System.Collections;
using System.Collections.Generic;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Mapping;

public interface ITireDealMapper
{
    TireDealModel ToModel(TireDealEntity entity);
    IEnumerable<TireDealModel> ToModel(IEnumerable<TireDealEntity> entities);
    TireDealEntity ToEntity(TireDealCreateModel model);
}