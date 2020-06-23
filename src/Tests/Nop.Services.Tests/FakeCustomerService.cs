using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Tests;

namespace Nop.Services.Tests
{
    public class FakeCustomerService : CustomerService
    {
        public FakeCustomerService(CachingSettings cachingSettings = null,
            CustomerSettings customerSettings = null,
            ICacheKeyService cacheKeyService = null,
            IEventPublisher eventPublisher = null,
            IGenericAttributeService genericAttributeService = null,
            IRepository<Address> customerAddressRepository = null,
            IRepository<Customer> customerRepository = null,
            IRepository<CustomerAddressMapping> customerAddressMappingRepository = null,
            IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository = null,
            IRepository<CustomerPassword> customerPasswordRepository = null,
            IRepository<CustomerRole> customerRoleRepository = null,
            IRepository<GenericAttribute> gaRepository = null,
            IRepository<ShoppingCartItem> shoppingCartRepository = null,
            IStoreContext storeContext = null,
            ShoppingCartSettings shoppingCartSettings = null) : base(
            cachingSettings ?? new CachingSettings(),
            customerSettings ?? new CustomerSettings(),
            cacheKeyService ?? new FakeCacheKeyService(),
            eventPublisher ?? new Mock<IEventPublisher>().Object,
            genericAttributeService ?? new Mock<IGenericAttributeService>().Object,
            customerAddressRepository.FakeRepoNullPropagation(),
            customerRepository.FakeRepoNullPropagation(),
            customerAddressMappingRepository.FakeRepoNullPropagation(),
            customerCustomerRoleMappingRepository.FakeRepoNullPropagation(),
            customerPasswordRepository.FakeRepoNullPropagation(),
            customerRoleRepository.FakeRepoNullPropagation(),
            gaRepository.FakeRepoNullPropagation(),
            shoppingCartRepository.FakeRepoNullPropagation(),
            new TestCacheManager(),
            storeContext ?? new Mock<IStoreContext>().Object,
            shoppingCartSettings ?? new ShoppingCartSettings())
        {
        }
    }
}
