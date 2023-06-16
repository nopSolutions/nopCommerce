using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.Deals.Models;
using Nop.Plugin.Widgets.Deals.Services;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Widgets.Deals.Mapping.Factories;

public class TireDealModelFactory : ITireDealModelFactory
{
    private readonly ITireDealService _tireDealService;
    private readonly ITireDealMapper _tireDealMapper;

    public TireDealModelFactory(ITireDealService tireDealService, ITireDealMapper tireDealMapper)
    {
        _tireDealService = tireDealService;
        _tireDealMapper = tireDealMapper;
    }

    public virtual async Task<TireDealListModel> PrepareTireDealListModelAsync(TireDealSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        var deals = (await _tireDealService.GetAllAsync()).ToPagedList(searchModel);
        
        var model = await new TireDealListModel().PrepareToGridAsync(searchModel, deals, () =>
        {
            return deals.SelectAwait(async deal => deal);
        });

        return model;
    }
}