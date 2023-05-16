using Nop.Core.Domain.Catalog;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.CustomAPI.Repository.IRepository
{
    public interface IProductRepository 
    {
        public IList<Product> GetAll();
        public Task<List<Product>> GetAllAsync(string? search);
        public Task<Product> GetByIdAsync(int id);
        public Task<ShoppingCartModel> GetCartItemAsync();
        public Task<ShoppingCartModel> AddCartItemAsync(Product product);

    }
}
