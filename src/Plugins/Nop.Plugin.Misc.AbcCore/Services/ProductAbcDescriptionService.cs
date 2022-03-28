using System.Linq;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class ProductAbcDescriptionService : IProductAbcDescriptionService
    {
        private readonly IRepository<ProductAbcDescription> _productAbcDescriptionRepository;

        public ProductAbcDescriptionService(
            IRepository<ProductAbcDescription> productAbcDescriptionRepository
        )
        {
            _productAbcDescriptionRepository = productAbcDescriptionRepository;
        }

        public async Task<ProductAbcDescription> GetProductAbcDescriptionByProductIdAsync(int productId)
        {
            var query = from pad in _productAbcDescriptionRepository.Table
                        where pad.Product_Id == productId
                        select pad;
            return await query.FirstOrDefaultAsync();
        }

        public async Task<ProductAbcDescription> GetProductAbcDescriptionByAbcItemNumberAsync(string abcitemNumber)
        {
            var query = from pad in _productAbcDescriptionRepository.Table
                        where pad.AbcItemNumber == abcitemNumber
                        select pad;
            return await query.FirstOrDefaultAsync();
        }
    }
}
