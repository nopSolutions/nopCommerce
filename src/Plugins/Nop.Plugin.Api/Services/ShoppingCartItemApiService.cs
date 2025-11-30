using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Infrastructure;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Services
{
    public class ShoppingCartItemApiService : IShoppingCartItemApiService
    {
        private readonly IRepository<ShoppingCartItem> _shoppingCartItemsRepository;
        private readonly IStoreContext _storeContext;

        public ShoppingCartItemApiService(IRepository<ShoppingCartItem> shoppingCartItemsRepository, IStoreContext storeContext)
        {
            _shoppingCartItemsRepository = shoppingCartItemsRepository;
            _storeContext = storeContext;
        }

        public List<ShoppingCartItem> GetShoppingCartItems(
            int? customerId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, int? limit = null,
            int? page = null, ShoppingCartType? shoppingCartType = null)
        {
            var query = GetShoppingCartItemsQuery(customerId, createdAtMin, createdAtMax,
                                                  updatedAtMin, updatedAtMax, shoppingCartType);

            return new ApiList<ShoppingCartItem>(query, (page ?? Constants.Configurations.DefaultPageValue) - 1, limit ?? Constants.Configurations.DefaultLimit);
        }

        public Task<ShoppingCartItem> GetShoppingCartItemAsync(int id)
        {
            return _shoppingCartItemsRepository.GetByIdAsync(id);
        }

        private IQueryable<ShoppingCartItem> GetShoppingCartItemsQuery(
            int? customerId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, ShoppingCartType? shoppingCartType = null)
        {
            var query = _shoppingCartItemsRepository.Table;

            if (customerId != null)
            {
                query = query.Where(shoppingCartItem => shoppingCartItem.CustomerId == customerId);
            }

            if (createdAtMin != null)
            {
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);
            }

            if (createdAtMax != null)
            {
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);
            }

            if (updatedAtMin != null)
            {
                query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);
            }

            if (updatedAtMax != null)
            {
                query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);
            }

            if (shoppingCartType != null)
            {
                query = query.Where(c => c.ShoppingCartTypeId == (int)shoppingCartType.Value);
            }

            // items for the current store only
            var currentStoreId = _storeContext.GetCurrentStore().Id;
            query = query.Where(c => c.StoreId == currentStoreId);

            query = query.OrderBy(shoppingCartItem => shoppingCartItem.Id);

            return query;
        }
    }
}
