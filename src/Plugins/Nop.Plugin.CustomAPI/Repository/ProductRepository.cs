using System.Reflection;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.CustomAPI.Repository.IRepository;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.CustomAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IRepository<Product> _productsRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

        public ProductRepository(IRepository<Product> productrepository, 
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IShoppingCartModelFactory shoppingCartModelFactory)
        {
            _productsRepository = productrepository;
            _storeContext = storeContext;
            _workContext = workContext;
            _shoppingCartService = shoppingCartService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
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

        public async Task<ShoppingCartModel> GetCartItemAsync()
        {
            var model = new ShoppingCartModel();
            var store = await _storeContext.GetCurrentStoreAsync();
            var carttype = ShoppingCartType.ShoppingCart;

            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), carttype, store.Id);

            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

            if(model == null)
            {
                return null;
            }

            return model;
        }


        public async Task<ShoppingCartModel> AddCartItemAsync(Product product)
        {
            var model = new ShoppingCartModel();
            var store = await _storeContext.GetCurrentStoreAsync();
            var carttype = ShoppingCartType.ShoppingCart;
           
            await _shoppingCartService.AddToCartAsync(await _workContext.GetCurrentCustomerAsync(), product, carttype, store.Id);

            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), carttype, store.Id);

            model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);

            return model;

        }

    }
}