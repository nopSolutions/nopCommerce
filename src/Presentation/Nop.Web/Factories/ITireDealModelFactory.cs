using Nop.Web.Areas.Admin.Models.TireDeals;

namespace Nop.Web.Factories;

public interface ITireDealModelFactory
{
    Task<TireDealListModel> PrepareTireDealListModelAsync(TireDealSearchModel searchModel);
}