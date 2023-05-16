using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.CustomAPI.Repository.IRepository
{
    public interface IProductRepository 
    {
        public IList<Product> GetAll();
        public Task<List<Product>> GetAllAsync(string? search);
        public Task<Product> GetByIdAsync(int id);

    }
}
