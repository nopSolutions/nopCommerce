using System.Threading.Tasks;
using Nop.Plugin.Widgets.Deals.Models;

namespace Nop.Plugin.Widgets.Deals.Mapping.Factories;

public interface ITireDealModelFactory
{
    Task<TireDealListModel> PrepareTireDealListModelAsync(TireDealSearchModel searchModel);
}