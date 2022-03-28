using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Services.Catalog;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework.Themes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    /// <summary>
    /// A service to retrive objects for the frontend views and manage caching
    /// </summary>
    public class FrontEndService
    {
        private const string CATEGORYISLEAF_KEY = "Nop.category.categoryisleaf.{0}";
        private const string THEMENAME_KEY = "Nop.currenttheme.name";
        private const string SEOSETTINGS_KEY = "Nop.settings.seosettings";
        private const string NONCLEARANCEURL_KEY = "Abc.nonclearanceurl";
        private const string STOREISCLEARANCE_KEY = "Abc.storeisclearance.{0}";
        private const string STOREISHAWTHORNE_KEY = "Abc.isHawthorne.{0}";
        private const string STOREISHAWTHORNECLEARANCE_KEY = "Abc.isHawthorneClearance.{0}";
        private const string PRODUCTISABC_KEY = ProductAbcDescription.PRODUCTABCDESCRIPTION_PATTERN_KEY + "isabc.{0}";
        private const string PRODUCTFLAG_KEY = ProductAbcDescription.PRODUCTABCDESCRIPTION_PATTERN_KEY + "productflag.{0}";
        private const string PRODUCTHASDOCUMENTS_KEY = ProductDocuments.DOCUMENT_PATTERN_KEY + "hasdocuments.{0}";
        private const string PACKAGE_PRODUCT_KEY = "Abc.packageproduct.{0}";
        private const string PRODUCT_SE_NAME_KEY = "Abc.sename.{0}";
        private const string PRODUCT_POPUP = "Abc.product.popup.{0}";

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductAttributeService _productAttributeService;

        public FrontEndService(
            IStaticCacheManager staticCacheManager,
            IProductService productService,
            IUrlRecordService urlRecordService,
            IProductAttributeService productAttributeService
        )
        {
            _staticCacheManager = staticCacheManager;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _productAttributeService = productAttributeService;
        }

        public async Task<string> GetThemeName()
        {
            return await _staticCacheManager.Get(new CacheKey(THEMENAME_KEY, "Abc."), async () =>
            {
                var themeContext = EngineContext.Current.Resolve<IThemeContext>();
                return await themeContext.GetWorkingThemeNameAsync();
            });
        }

        public bool StoreIsClearance(Store currentStore)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(STOREISCLEARANCE_KEY, currentStore.Id), "Abc."), () =>
            {
                string abcClearance = "clearance";
                return currentStore.Url.Contains(abcClearance);
            });
        }

        public bool StoreIsHawthorne(Store currentStore)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(STOREISHAWTHORNE_KEY, currentStore.Id), "Abc."), () =>
            {
                string hawthorneIdentifier = "hawthorne";
                return currentStore.Url.Contains(hawthorneIdentifier);
            });
        }

        public bool StoreIsHawthorneClearance(Store currentStore)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(STOREISHAWTHORNECLEARANCE_KEY, currentStore.Id), "Abc."), () =>
            {
                string hawthorneIdentifier = "clearance.hawthorne";
                return currentStore.Url.Contains(hawthorneIdentifier);
            });
        }

        public async Task<string> GetNonClearanceUrl(int currentStoreId)
        {
            return await _staticCacheManager.Get(new CacheKey(NONCLEARANCEURL_KEY, "Abc."), async () =>
            {
                var storeService = EngineContext.Current.Resolve<IStoreService>();
                var storeList = await storeService.GetAllStoresAsync();
                //this grabs the non-clearance store
                Store abcStore = storeList.Where(s => s.Id != currentStoreId).Select(s => s).FirstOrDefault();
                return abcStore.Url;
            });
        }

        public SeoSettings GetSeoSettings()
        {
            return _staticCacheManager.Get(new CacheKey(SEOSETTINGS_KEY, "Abc."), () =>
            {
                return EngineContext.Current.Resolve<SeoSettings>();
            });
        }

        public bool CategoryHasChildren(int categoryId)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(CATEGORYISLEAF_KEY, categoryId), "Abc."),
                () => { return EngineContext.Current.Resolve<IRepository<Category>>().Table.Any(c => c.ParentCategoryId == categoryId && c.Published == true && c.Deleted == false); });
        }

        public ProductAbcDescription GetProductAbcDescriptionByProductId(int productId)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(ProductAbcDescription.PRODUCTABCDESCRIPTION_BY_PRODID_KEY, productId), "Abc."),
                ProductAbcDescription.GetByProductIdFunc(EngineContext.Current.Resolve<IRepository<ProductAbcDescription>>(), productId));
        }

        public bool ProductIsAbc(int productId)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(PRODUCTISABC_KEY, productId), "Abc."),
                () => { return ProductAbcDescription.GetByProductIdFunc(EngineContext.Current.Resolve<IRepository<ProductAbcDescription>>(), productId)() != null; });
        }
        public ProductFlag GetProductFlag(int productId)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(PRODUCTFLAG_KEY, productId), "Abc."), () =>
            {
                var productFlagRepository = EngineContext.Current.Resolve<IRepository<ProductFlag>>();
                return productFlagRepository.Table
                    .Where(pf => pf.ProductId == productId)
                    .Select(pf => pf).FirstOrDefault();
            });
        }

        public string GetDocumentsByProductId(int productId)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(ProductDocuments.DOCUMENT_BY_PRODID_KEY, productId), "Abc."),
                ProductDocuments.GetByProductIdFunc(EngineContext.Current.Resolve<IRepository<ProductDocuments>>(), productId));
        }

        public bool ProductHasDocuments(int productId)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(PRODUCTHASDOCUMENTS_KEY, productId), "Abc."),
                () => { return ProductDocuments.GetByProductIdFunc(EngineContext.Current.Resolve<IRepository<ProductDocuments>>(), productId)() != null; });
        }

        public Task<string> GetProductSeNameById(int productId)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(PRODUCT_SE_NAME_KEY, productId), "Abc."), async () =>
            {
                var product = await _productService.GetProductByIdAsync(productId);
                return await _urlRecordService.GetSeNameAsync(product);
            });
        }

        public bool IsProductPopup(int productId)
        {
            return _staticCacheManager.Get(new CacheKey(string.Format(PRODUCT_POPUP, productId), "Abc."), () =>
            {
                return ProductRequiresLogin.GetByProductIdFunc(EngineContext.Current.Resolve<IRepository<ProductRequiresLogin>>(), productId)() != null;
            });
        }
    }
}
