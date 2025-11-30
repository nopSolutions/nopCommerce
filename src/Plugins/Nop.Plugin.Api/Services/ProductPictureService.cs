using System.Linq;
using Nop.Data;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Api.Services
{
    public class ProductPictureService : IProductPictureService
    {
        private readonly IRepository<ProductPicture> _productPictureRepository;

        public ProductPictureService(IRepository<ProductPicture> productPictureRepository)
        {
            _productPictureRepository = productPictureRepository;
        }

        public ProductPicture GetProductPictureByPictureId(int pictureId)
        {
            if (pictureId == 0)
            {
                return null;
            }

            var query = from pp in _productPictureRepository.Table
                        where pp.PictureId == pictureId
                        select pp;

            var productPictures = query.ToList();

            return productPictures.FirstOrDefault();
        }
    }
}
