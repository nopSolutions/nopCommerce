using Nop.Core.Domain.TireDeals;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Models.TireDeals;

namespace Nop.Web.Mapping;

public class TireDealMapper : ITireDealMapper
{
    private readonly IPictureService _pictureService;

    public TireDealMapper(IPictureService pictureService)
    {
        _pictureService = pictureService;
    }

    public TireDealModel ToModel(TireDeal entity)
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

    public IList<TireDealModel> ToModel(IEnumerable<TireDeal> entities)
    {
        return entities.Select(ToModel).ToList();
    }

    public TireDeal ToEntity(TireDealCreateModel model)
    {
        return new TireDeal()
        {
            Title = model.Title,
            LongDescription = model.LongDescription,
            ShortDescription = model.ShortDescription,
            IsActive = model.IsActive,
            BrandPictureId = model.BrandPictureId,
            BackgroundPictureId = model.BackgroundPictureId
        };
    }

    public TireDeal ToEntity(TireDealUpdateModel model)
    {
        return new TireDeal()
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