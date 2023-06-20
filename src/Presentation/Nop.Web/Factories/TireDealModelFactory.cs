using AutoMapper;
using Nop.Data.Extensions;
using Nop.Services.TireDeals;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.TireDeals;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Factories;

public class TireDealModelFactory : ITireDealModelFactory
{
    private readonly ITireDealService _tireDealService;

    public TireDealModelFactory(ITireDealService tireDealService)
    {
        _tireDealService = tireDealService;
    }

    public virtual async Task<TireDealListModel> PrepareTireDealListModelAsync(TireDealSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        //get topics
        var deals = await _tireDealService.GetAllAsync(title: searchModel.SearchTireDealTitle);

        var pagedDeals = deals.ToPagedList(searchModel);

        //prepare grid model
        var model = await new TireDealListModel().PrepareToGridAsync(searchModel, pagedDeals, () =>
        {
            return pagedDeals.SelectAwait(async deal =>
            {
                //fill in model values from the entity
                var dealModel = deal.ToModel<TireDealModel>();

                return dealModel;
            });
        });

        return model;
    }
}