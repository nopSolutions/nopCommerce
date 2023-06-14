using System.Collections;
using System.Collections.Generic;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Mapping;

public interface IDealMapper
{
    DealModel ToModel(DealEntity entity);
    IEnumerable<DealModel> ToModel(IEnumerable<DealEntity> entities);
    DealEntity ToEntity(DealModel model);
}