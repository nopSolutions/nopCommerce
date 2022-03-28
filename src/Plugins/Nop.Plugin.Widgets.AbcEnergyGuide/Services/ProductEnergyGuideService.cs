using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Data;
using System.Linq;

namespace Nop.Plugin.Widgets.AbcEnergyGuide.Services
{
    public class ProductEnergyGuideService : IProductEnergyGuideService
    {
        private readonly IRepository<ProductEnergyGuide> _productEnergyGuideRepository;

        public ProductEnergyGuideService(
            IRepository<ProductEnergyGuide> productEnergyGuideRepository
        )
        {
            _productEnergyGuideRepository = productEnergyGuideRepository;
        }

        public ProductEnergyGuide GetEnergyGuideByProductId(int productId)
        {
            return _productEnergyGuideRepository.Table
                                                .Where(peg => peg.ProductId == productId)
                                                .FirstOrDefault();
        }
    }
}