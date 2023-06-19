using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Mapping;

public interface ITireDealMapper
{
    TireDealModel ToModel(TireDealEntity entity);
    IList<TireDealModel> ToModel(IEnumerable<TireDealEntity> entities);
    TireDealEntity ToEntity(TireDealCreateModel model);
    TireDealEntity ToEntity(TireDealUpdateModel model);
    Task<IList<PublicInfoModel>> ToModel(IList<TireDealModel> models);
}