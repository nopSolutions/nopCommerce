using System.Globalization;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Services.Installation
{
    /// <summary>
    /// Installation service
    /// </summary>
    public partial class InstallationService : IInstallationService
    {
        #region Fields

        protected readonly INopDataProvider _dataProvider;
        protected readonly INopFileProvider _fileProvider;
        protected readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        protected readonly IRepository<Address> _addressRepository;
        protected readonly IRepository<Category> _categoryRepository;
        protected readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        protected readonly IRepository<Country> _countryRepository;
        protected readonly IRepository<Currency> _currencyRepository;
        protected readonly IRepository<Customer> _customerRepository;
        protected readonly IRepository<CustomerRole> _customerRoleRepository;
        protected readonly IRepository<DeliveryDate> _deliveryDateRepository;
        protected readonly IRepository<EmailAccount> _emailAccountRepository;
        protected readonly IRepository<Language> _languageRepository;
        protected readonly IRepository<Manufacturer> _manufacturerRepository;
        protected readonly IRepository<ManufacturerTemplate> _manufacturerTemplateRepository;
        protected readonly IRepository<MeasureDimension> _measureDimensionRepository;
        protected readonly IRepository<MeasureWeight> _measureWeightRepository;
        protected readonly IRepository<Product> _productRepository;
        protected readonly IRepository<ProductAttribute> _productAttributeRepository;
        protected readonly IRepository<ProductAvailabilityRange> _productAvailabilityRangeRepository;
        protected readonly IRepository<ProductTag> _productTagRepository;
        protected readonly IRepository<ProductTemplate> _productTemplateRepository;
        protected readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        protected readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        protected readonly IRepository<StateProvince> _stateProvinceRepository;
        protected readonly IRepository<Store> _storeRepository;
        protected readonly IRepository<TaxCategory> _taxCategoryRepository;
        protected readonly IRepository<TopicTemplate> _topicTemplateRepository;
        protected readonly IRepository<UrlRecord> _urlRecordRepository;
        protected readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public InstallationService(INopDataProvider dataProvider,
            INopFileProvider fileProvider,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<Address> addressRepository,
            IRepository<Category> categoryRepository,
            IRepository<CategoryTemplate> categoryTemplateRepository,
            IRepository<Country> countryRepository,
            IRepository<Currency> currencyRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<DeliveryDate> deliveryDateRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<Language> languageRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<ManufacturerTemplate> manufacturerTemplateRepository,
            IRepository<MeasureDimension> measureDimensionRepository,
            IRepository<MeasureWeight> measureWeightRepository,
            IRepository<Product> productRepository,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAvailabilityRange> productAvailabilityRangeRepository,
            IRepository<ProductTag> productTagRepository,
            IRepository<ProductTemplate> productTemplateRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<Store> storeRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            IRepository<TopicTemplate> topicTemplateRepository,
            IRepository<UrlRecord> urlRecordRepository,
            IWebHelper webHelper)
        {
            _dataProvider = dataProvider;
            _fileProvider = fileProvider;
            _activityLogTypeRepository = activityLogTypeRepository;
            _addressRepository = addressRepository;
            _categoryRepository = categoryRepository;
            _categoryTemplateRepository = categoryTemplateRepository;
            _countryRepository = countryRepository;
            _currencyRepository = currencyRepository;
            _customerRepository = customerRepository;
            _customerRoleRepository = customerRoleRepository;
            _deliveryDateRepository = deliveryDateRepository;
            _emailAccountRepository = emailAccountRepository;
            _languageRepository = languageRepository;
            _manufacturerRepository = manufacturerRepository;
            _manufacturerTemplateRepository = manufacturerTemplateRepository;
            _measureDimensionRepository = measureDimensionRepository;
            _measureWeightRepository = measureWeightRepository;
            _productAttributeRepository = productAttributeRepository;
            _productAvailabilityRangeRepository = productAvailabilityRangeRepository;
            _productRepository = productRepository;
            _productTagRepository = productTagRepository;
            _productTemplateRepository = productTemplateRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _storeRepository = storeRepository;
            _taxCategoryRepository = taxCategoryRepository;
            _topicTemplateRepository = topicTemplateRepository;
            _urlRecordRepository = urlRecordRepository;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<T> InsertInstallationDataAsync<T>(T entity) where T : BaseEntity
        {
            return await _dataProvider.InsertEntityAsync(entity);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertInstallationDataAsync<T>(params T[] entities) where T : BaseEntity
        {
            await _dataProvider.BulkInsertEntitiesAsync(entities);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertInstallationDataAsync<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            if (!entities.Any())
                return;

            await InsertInstallationDataAsync(entities.ToArray());
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task UpdateInstallationDataAsync<T>(T entity) where T : BaseEntity
        {
            await _dataProvider.UpdateEntityAsync(entity);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task UpdateInstallationDataAsync<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            if (!entities.Any())
                return;

            foreach (var entity in entities)
                await _dataProvider.UpdateEntityAsync(entity);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<int> GetSpecificationAttributeOptionIdAsync(string specAttributeName, string specAttributeOptionName)
        {
            var specificationAttribute = await _specificationAttributeRepository.Table
                .SingleAsync(sa => sa.Name == specAttributeName);

            var specificationAttributeOption = await _specificationAttributeOptionRepository.Table
                .SingleAsync(sao => sao.Name == specAttributeOptionName && sao.SpecificationAttributeId == specificationAttribute.Id);

            return specificationAttributeOption.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="fileName"></param>
        /// <param name="displayOrder"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the identifier of inserted picture
        /// </returns>
        protected virtual async Task<int> InsertProductPictureAsync(Product product, string fileName, int displayOrder = 1)
        {
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = GetSamplesPath();

            var pic = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, fileName)), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(product.Name));

            await InsertInstallationDataAsync(
                new ProductPicture
                {
                    ProductId = product.Id,
                    PictureId = pic.Id,
                    DisplayOrder = displayOrder
                });

            return pic.Id;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<string> ValidateSeNameAsync<T>(T entity, string seName) where T : BaseEntity
        {
            //duplicate of ValidateSeName method of \Nop.Services\Seo\UrlRecordService.cs (we cannot inject it here)
            ArgumentNullException.ThrowIfNull(entity);

            //validation
            var okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            seName = seName.Trim().ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (var c in seName.ToCharArray())
            {
                var c2 = c.ToString();
                if (okChars.Contains(c2))
                    sb.Append(c2);
            }

            seName = sb.ToString();
            seName = seName.Replace(" ", "-");
            while (seName.Contains("--"))
                seName = seName.Replace("--", "-");
            while (seName.Contains("__"))
                seName = seName.Replace("__", "_");

            //max length
            seName = CommonHelper.EnsureMaximumLength(seName, NopSeoDefaults.SearchEngineNameLength);

            //ensure this seName is not reserved yet
            var i = 2;
            var tempSeName = seName;
            while (true)
            {
                //check whether such slug already exists (and that is not the current entity)

                var query = from ur in _urlRecordRepository.Table
                            where tempSeName != null && ur.Slug == tempSeName
                            select ur;
                var urlRecord = await query.FirstOrDefaultAsync();

                var entityName = entity.GetType().Name;
                var reserved = urlRecord != null && !(urlRecord.EntityId == entity.Id && urlRecord.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
                if (!reserved)
                    break;

                tempSeName = $"{seName}-{i}";
                i++;
            }

            seName = tempSeName;

            return seName;
        }

        protected virtual string GetSamplesPath()
        {
            return _fileProvider.GetAbsolutePath(NopInstallationDefaults.SampleImagesPath);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Install required data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <param name="defaultUserPassword">Default user password</param>
        /// <param name="languagePackInfo">Language pack info</param>
        /// <param name="regionInfo">RegionInfo</param>
        /// <param name="cultureInfo">CultureInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InstallRequiredDataAsync(string defaultUserEmail, string defaultUserPassword,
            (string languagePackDownloadLink, int languagePackProgress) languagePackInfo, RegionInfo regionInfo, CultureInfo cultureInfo)
        {
            await InstallStoresAsync();
            await InstallMeasuresAsync(regionInfo);
            await InstallTaxCategoriesAsync();
            await InstallLanguagesAsync(languagePackInfo, cultureInfo, regionInfo);
            await InstallCurrenciesAsync(cultureInfo, regionInfo);
            await InstallCountriesAndStatesAsync();
            await InstallShippingMethodsAsync();
            await InstallDeliveryDatesAsync();
            await InstallProductAvailabilityRangesAsync();
            await InstallEmailAccountsAsync();
            await InstallMessageTemplatesAsync();
            await InstallTopicTemplatesAsync();
            await InstallSettingsAsync(regionInfo);
            await InstallCustomersAndUsersAsync(defaultUserEmail, defaultUserPassword);
            await InstallTopicsAsync();
            await InstallActivityLogTypesAsync();
            await InstallProductTemplatesAsync();
            await InstallCategoryTemplatesAsync();
            await InstallManufacturerTemplatesAsync();
            await InstallScheduleTasksAsync();
            await InstallReturnRequestReasonsAsync();
            await InstallReturnRequestActionsAsync();
        }

        /// <summary>
        /// Install sample data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InstallSampleDataAsync(string defaultUserEmail)
        {
            await InstallSampleCustomersAsync();
            await InstallCheckoutAttributesAsync();
            await InstallSpecificationAttributesAsync();
            await InstallProductAttributesAsync();
            await InstallCategoriesAsync();
            await InstallManufacturersAsync();
            await InstallProductsAsync(defaultUserEmail);
            await InstallForumsAsync();
            await InstallDiscountsAsync();
            await InstallBlogPostsAsync(defaultUserEmail);
            await InstallNewsAsync(defaultUserEmail);
            await InstallPollsAsync();
            await InstallWarehousesAsync();
            await InstallVendorsAsync();
            await InstallAffiliatesAsync();
            await InstallOrdersAsync();
            await InstallActivityLogAsync(defaultUserEmail);
            await InstallSearchTermsAsync();

            var settingService = EngineContext.Current.Resolve<ISettingService>();

            await settingService.SaveSettingAsync(new DisplayDefaultMenuItemSettings
            {
                DisplayHomepageMenuItem = false,
                DisplayNewProductsMenuItem = false,
                DisplayProductSearchMenuItem = false,
                DisplayCustomerInfoMenuItem = false,
                DisplayBlogMenuItem = false,
                DisplayForumsMenuItem = false,
                DisplayContactUsMenuItem = false
            });
        }

        #endregion
    }
}