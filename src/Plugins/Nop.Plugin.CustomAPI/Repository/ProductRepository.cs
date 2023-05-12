using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.CustomAPI.Repository.IRepository;

namespace Nop.Plugin.CustomAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IRepository<Product> _productsRepository;

        public ProductRepository(IRepository<Product> productrepository)
        {
            _productsRepository = productrepository;
        }

        public IList<Product> GetAll()
        {
            var products =  _productsRepository.GetAll();
            return products;
        }

        public async Task<List<Product>> GetAllAsync(string? search)
        {
            var products =  _productsRepository.GetAll().Where(x => x.Name.Contains(search) || x.Sku.Contains(search)).ToList();
            return products;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var products = await _productsRepository.GetByIdAsync(id);
            return products;
        }

    }
}
