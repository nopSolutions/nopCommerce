using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.SqlQuery;
using Nop.Plugin.Widgets.Deals.Domain;
using Nop.Plugin.Widgets.Deals.Models;
using Nop.Services.Media;

namespace Nop.Plugin.Widgets.Deals.Mapping;

public class TireDealMapper : ITireDealMapper
{
    private readonly IPictureService _pictureService;

    public TireDealMapper(IPictureService pictureService)
    {
        _pictureService = pictureService;
    }

    public TireDealModel ToModel(TireDealEntity entity)
    {
        return new TireDealModel()
        {
            Id = entity.Id,
            Title = entity.Title,
            LongDescription = entity.LongDescription,
            ShortDescription = entity.ShortDescription,
            IsActive = entity.IsActive,
            BrandPictureId = entity.BrandPictureId,
            BackgroundPictureId = entity.BackgroundPictureId
        };
    }

    public IList<TireDealModel> ToModel(IEnumerable<TireDealEntity> entities)
    {
        return entities.Select(ToModel).ToList();
    }

    public TireDealEntity ToEntity(TireDealCreateModel model)
    {
        return new TireDealEntity()
        {
            Title = model.Title,
            LongDescription = model.LongDescription,
            ShortDescription = model.ShortDescription,
            IsActive = model.IsActive,
            BrandPictureId = model.BrandPictureId,
            BackgroundPictureId = model.BackgroundPictureId
        };
    }

    public TireDealEntity ToEntity(TireDealUpdateModel model)
    {
        return new TireDealEntity()
        {
            Title = model.Title,
            LongDescription = model.LongDescription,
            ShortDescription = model.ShortDescription,
            IsActive = model.IsActive,
            BrandPictureId = model.BrandPictureId,
            BackgroundPictureId = model.BackgroundPictureId
        };
    }

    public async Task<IList<PublicInfoModel>> ToModel(IList<TireDealModel> models)
    {
        List<PublicInfoModel> publicModels = new();
        
        foreach (var model in models)
        {
            publicModels.Add(new PublicInfoModel()
            {
                Id = model.Id,
                Title = model.Title,
                LongDescription = model.LongDescription,
                ShortDescription = model.ShortDescription,
                IsActive = model.IsActive,
                BrandPictureUrl = await _pictureService.GetPictureUrlAsync(model.BrandPictureId),
                BackgroundPictureUrl = await _pictureService.GetPictureUrlAsync(model.BackgroundPictureId)
            });
        }

        return publicModels;
    }
}