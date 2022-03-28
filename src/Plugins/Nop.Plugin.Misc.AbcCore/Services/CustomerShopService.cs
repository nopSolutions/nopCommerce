using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using System.Linq;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class CustomerShopService : ICustomerShopService
    {
        private readonly IRepository<CustomerShopMapping> _customerShopMappingRepository;

        public CustomerShopService(
            IRepository<CustomerShopMapping> customerShopMappingRepository)
        {
            _customerShopMappingRepository = customerShopMappingRepository;
        }

        public CustomerShopMapping GetCurrentCustomerShopMapping(int customerId)
        {
            var query = _customerShopMappingRepository.Table
                .Where(csm => csm.CustomerId == customerId)
                .Select(csm => csm);
            return query.ToList().FirstOrDefault();
        }

        public void InsertOrUpdateCustomerShop(int customerId, int shopId)
        {
            CustomerShopMapping csm = GetCurrentCustomerShopMapping(customerId);

            if (csm == null)
            {
                csm = new CustomerShopMapping();
                csm.CustomerId = customerId;
                csm.ShopId = shopId;

                _customerShopMappingRepository.InsertAsync(csm);
            }
            else
            {
                csm.ShopId = shopId;
                _customerShopMappingRepository.UpdateAsync(csm);
            }
        }
    }
}
