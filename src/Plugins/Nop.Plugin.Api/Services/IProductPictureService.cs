using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Api.Services
{
    public interface IProductPictureService
    {
        ProductPicture GetProductPictureByPictureId(int pictureId);
    }
}
