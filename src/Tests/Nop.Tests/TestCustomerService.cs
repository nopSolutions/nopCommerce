using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Tests
{
    public static class TestCustomerService
    {
        public static ICustomerService Get(
            CustomerSettings customerSettings = null,
                Mock<IDataProvider> dataProvider = null,
                Mock<IDbContext> dbContext = null,
                Mock<IEventPublisher> eventPublisher = null,
                Mock<IGenericAttributeService> genericAttributeService = null,
                Mock<IRepository<Address>> customerAddressRepository = null,
                Mock<IRepository<Customer>> customerRepository = null,
                Mock<IRepository<CustomerAddressMapping>> customerAddressMappingRepository = null,
                Mock<IRepository<CustomerCustomerRoleMapping>> customerCustomerRoleMappingRepository = null,
                Mock<IRepository<CustomerPassword>> customerPasswordRepository = null,
                Mock<IRepository<CustomerRole>> customerRoleRepository = null,
                Mock<IRepository<GenericAttribute>> gaRepository = null,
                Mock<IRepository<ShoppingCartItem>> shoppingCartRepository = null,
                ShoppingCartSettings shoppingCartSettings = null)
        {
            return new CustomerServiceGenerator().Setup(
                customerSettings,
                dataProvider, 
                dbContext, 
                eventPublisher, 
                genericAttributeService, 
                customerAddressRepository,
                customerRepository,
                customerAddressMappingRepository,
                customerCustomerRoleMappingRepository,
                customerPasswordRepository,
                customerRoleRepository,
                gaRepository,
                shoppingCartRepository,
                shoppingCartSettings);
        }

        protected class CustomerServiceGenerator
        {
            private Dictionary<Type, IList> _store = new Dictionary<Type, IList>();

            protected void SetupRepo<T>(ref Mock<IRepository<T>> mockRepo, IEnumerable<T> initData = null) where T : BaseEntity
            {

                if (mockRepo is null)
                {
                    mockRepo = new Mock<IRepository<T>>();

                    SetupGRUD(mockRepo, RepoTable<T>(), (T ins) => Insert(ins), (T ins) => Delete(ins));
                }

                if (initData != null)
                    Insert(initData);
            }

            protected IQueryable<T> RepoTable<T>() where T : BaseEntity
            {
                if (!_store.ContainsKey(typeof(T)))
                    _store.Add(typeof(T), new List<T>());

                return (_store[typeof(T)] as List<T>)?.AsQueryable();
            }

            protected void Insert<T>(T entity) where T : BaseEntity
            {
                if (!_store.ContainsKey(typeof(T)))
                    _store.Add(typeof(T), new List<T>());

                _store[typeof(T)].Add(entity);
            }

            protected void Insert<T>(IEnumerable<T> entities) where T : BaseEntity
            {
                if (entities is null)
                    throw new ArgumentException(nameof(entities));

                foreach (var item in entities)
                {
                    Insert(item);
                }
            }

            protected void Delete<T>(T entity) where T : BaseEntity
            {
                if (!_store.ContainsKey(typeof(T)))
                    _store.Add(typeof(T), new List<T>());

                var listItem = (_store[typeof(T)] as List<T>).Find(x => x.Id == entity.Id);

                if (listItem != null)
                    _store[typeof(T)].Remove(entity);
            }

            protected T GetById<T>(int id) where T : BaseEntity
            {
                if (!_store.ContainsKey(typeof(T)))
                    _store.Add(typeof(T), new List<T>());

                return (_store[typeof(T)] as List<T>).Find(x => x.Id == id);                
            }

            protected void SetupGRUD<T>(Mock<IRepository<T>> mockRepo, IQueryable<T> returns, Action<T> insertAction, Action<T> deleteAction) 
                where T : BaseEntity
            {
                mockRepo.Setup(r => r.Table).Returns(returns);
                mockRepo.Setup(r => r.Insert(It.IsAny<T>())).Callback(insertAction);
                mockRepo.Setup(r => r.Delete(It.IsAny<T>())).Callback(deleteAction);
                mockRepo.Setup(r => r.GetById(It.Is<int>(x => x > 0))).Returns((int id) => GetById<T>(id));
            }

            public ICustomerService Setup(
                CustomerSettings customerSettings,
                Mock<IDataProvider> dataProvider,
                Mock<IDbContext> dbContext,
                Mock<IEventPublisher> eventPublisher,
                Mock<IGenericAttributeService> genericAttributeService,
                Mock<IRepository<Address>> customerAddressRepository,
                Mock<IRepository<Customer>> customerRepository,
                Mock<IRepository<CustomerAddressMapping>> customerAddressMappingRepository,
                Mock<IRepository<CustomerCustomerRoleMapping>> customerCustomerRoleMappingRepository,
                Mock<IRepository<CustomerPassword>> customerPasswordRepository,
                Mock<IRepository<CustomerRole>> customerRoleRepository,
                Mock<IRepository<GenericAttribute>> gaRepository,
                Mock<IRepository<ShoppingCartItem>> shoppingCartRepository,
                ShoppingCartSettings shoppingCartSettings)
            {
                SetupRepo(ref customerAddressRepository);
                SetupRepo(ref customerRepository);
                SetupRepo(ref customerAddressMappingRepository);
                SetupRepo(ref customerCustomerRoleMappingRepository);
                SetupRepo(ref customerPasswordRepository);
                SetupRepo(ref customerRoleRepository);
                SetupRepo(ref gaRepository);
                SetupRepo(ref shoppingCartRepository);

                return new CustomerService(
                    customerSettings ?? new CustomerSettings(),
                    new TestCacheManager(),
                    (dataProvider ?? new Mock<IDataProvider>()).Object,
                    (dbContext ?? new Mock<IDbContext>()).Object, 
                    (eventPublisher ?? new Mock<IEventPublisher>()).Object,
                    (genericAttributeService ?? new Mock<IGenericAttributeService>()).Object,
                    customerAddressRepository.Object, 
                    customerRepository.Object,
                    customerAddressMappingRepository.Object,
                    customerCustomerRoleMappingRepository.Object,
                    customerPasswordRepository.Object, 
                    customerRoleRepository.Object, 
                    gaRepository.Object,
                    shoppingCartRepository.Object, 
                    new TestCacheManager(), 
                    shoppingCartSettings ?? new ShoppingCartSettings());
            }
        }
    }
}
