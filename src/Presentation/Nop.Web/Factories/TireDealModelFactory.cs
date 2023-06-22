using AutoMapper;
using DocumentFormat.OpenXml.Office2013.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Data.Extensions;
using Nop.Services.Media;
using Nop.Services.TireDeals;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.TireDeals;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Factories;

public class TireDealModelFactory : ITireDealModelFactory
{
    private readonly ITireDealService _tireDealService;
    private readonly IPictureService _pictureService;

    public TireDealModelFactory(ITireDealService tireDealService, IPictureService pictureService)
    {
        _tireDealService = tireDealService;
        _pictureService = pictureService;
    }

    public virtual async Task<TireDealListModel> PrepareTireDealListModelAsync(TireDealSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        //get deals
        var deals = await _tireDealService.GetAllAsync(
            title: searchModel.SearchTireDealTitle, 
            isActive: searchModel.SearchTireDealIsActive,
            id: searchModel.SearchTireDealId);

        var pagedDeals = deals.ToPagedList(searchModel);

        //prepare grid model
        var model = await new TireDealListModel().PrepareToGridAsync(searchModel, pagedDeals, () =>
        {
            return pagedDeals.SelectAwait(async deal =>
            {
                //fill in model values from the entity
                var dealModel = deal.ToModel<TireDealModel>();
                
                dealModel.BackgroundPictureUrl = await _pictureService.GetPictureUrlAsync(dealModel.BackgroundPictureId);
                
                return dealModel;
            });
        });

        return model;
    }

    public TireDealSearchModel PrepareTireDealSearchModelAsync()
    {
        var searchModel = new TireDealSearchModel() { AvailablePageSizes = "5, 10, 50, 100"};
        
        searchModel.AvailableActiveOptions.Add(new SelectListItem { Value = "null", Text = "All" });
        searchModel.AvailableActiveOptions.Add(new SelectListItem { Value = "true", Text = "Active" });
        searchModel.AvailableActiveOptions.Add(new SelectListItem { Value = "false", Text = "Inactive" });

        return searchModel;
    }
}