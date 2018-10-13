using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Data.MySQL.Services.Catalog
{
    public class CategoryService : Nop.Services.Catalog.CategoryService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IWorkContext _workContext;
        private readonly string _entityName;

        #endregion

        #region Ctor

        public CategoryService(CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            IAclService aclService,
            ICacheManager cacheManager,
            IDataProvider dataProvider,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IRepository<AclRecord> aclRepository,
            IRepository<Category> categoryRepository,
            IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext) : base(catalogSettings,
            commonSettings,
            aclService,
            cacheManager,
            dataProvider,
            dbContext,
            eventPublisher,
            localizationService,
            aclRepository,
            categoryRepository,
            productRepository,
            productCategoryRepository,
            storeMappingRepository,
            staticCacheManager,
            storeContext,
            storeMappingService,
            workContext)
        {
            _catalogSettings = catalogSettings;
            _aclRepository = aclRepository;
            _categoryRepository = categoryRepository;
            _storeMappingRepository = storeMappingRepository;
            _workContext = workContext;
            this._entityName = typeof(Category).Name;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public override IPagedList<Category> GetAllCategories(string categoryName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _categoryRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!string.IsNullOrWhiteSpace(categoryName))
                query = query.Where(c => c.Name.Contains(categoryName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
            {
                if (!showHidden && !_catalogSettings.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                    query = from c in query
                            join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = _entityName } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                            from acl in c_acl.DefaultIfEmpty()
                            where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                            select c;
                }

                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    query = from c in query
                            join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                            from sm in c_sm.DefaultIfEmpty()
                            where !c.LimitedToStores || storeId == sm.StoreId
                            select c;
                }

                query = query.Distinct().OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            }

            var unsortedCategories = query.ToList();

            //sort categories
            var sortedCategories = this.SortCategoriesForTree(unsortedCategories);

            //paging
            return new PagedList<Category>(sortedCategories, pageIndex, pageSize);
        }

        #endregion
    }
}
