using Nop.Core.Domain.TireDeals;
using Nop.Web.Areas.Admin.Models.TireDeals;

namespace Nop.Web.Mapping;

public interface ITireDealMapper
{
    TireDealModel ToModel(TireDeal entity);
    IList<TireDealModel> ToModel(IEnumerable<TireDeal> entities);
    TireDeal ToEntity(TireDealCreateModel model);
    TireDeal ToEntity(TireDealUpdateModel model);
    Task<IList<PublicInfoModel>> ToModel(IList<TireDealModel> models);
}