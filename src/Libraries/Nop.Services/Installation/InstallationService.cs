using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Seo;

namespace Nop.Services.Installation;

/// <summary>
/// Installation service
/// </summary>
public partial class InstallationService : IInstallationService
{
    #region Fields

    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly INopDataProvider _dataProvider;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IWebHelper _webHelper;

    protected InstallationSettings _installationSettings;

    #endregion

    #region Ctor

    public InstallationService(IHttpClientFactory httpClientFactory,
        INopDataProvider dataProvider,
        INopFileProvider fileProvider,
        IWebHelper webHelper)
    {
        _httpClientFactory = httpClientFactory;
        _dataProvider = dataProvider;
        _fileProvider = fileProvider;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets default customer identifier
    /// </summary>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the identifier of default customer</returns>
    protected virtual async Task<int> GetDefaultCustomerIdAsync()
    {
        if (_defaultCustomerId.HasValue)
            return _defaultCustomerId.Value;

        var customer = await Table<Customer>().FirstOrDefaultAsync(x => x.Email == _installationSettings.AdminEmail) ?? throw new Exception("Cannot load default customer");

        _defaultCustomerId = customer.Id;

        return customer.Id;
    }

    /// <summary>
    /// Returns queryable source for specified mapping class for current connection,
    /// mapped to database table or view.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Queryable source</returns>
    protected virtual IQueryable<TEntity> Table<TEntity>() where TEntity : BaseEntity
    {
        return _dataProvider.GetTable<TEntity>();
    }

    /// <summary>
    /// Gets the first entity identifier
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the entity identifier or null, if entity is not exists</returns>
    protected virtual async Task<int?> GetFirstEntityIdAsync<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : BaseEntity
    {
        var entity = await Table<TEntity>().FirstOrDefaultAsync(predicate ?? (_ => true));

        return entity?.Id;
    }

    /// <summary>
    /// Get the entity entry by identifier
    /// </summary>
    /// <param name="id">Entity entry identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entry
    /// </returns>
    protected virtual async Task<TEntity> GetByIdAsync<TEntity>(int? id) where TEntity : BaseEntity
    {
        if (!id.HasValue || id == 0)
            return null;

        return await Table<TEntity>().FirstOrDefaultAsync(entity => entity.Id == id.Value);
    }

    /// <summary>
    /// Validate search engine name
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="seName">Search engine name to validate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the valid seName
    /// </returns>
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

            var currentSeName = tempSeName;
            var query = Table<UrlRecord>().Where(ur => currentSeName != null && ur.Slug == currentSeName);
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

    #endregion

    #region Methods

    /// <summary>
    /// Install
    /// </summary>
    /// <param name="installationSettings">Installation settings</param>
    public virtual async Task InstallAsync(InstallationSettings installationSettings)
    {
        _installationSettings = installationSettings;
        _defaultLanguageId = null;
        _defaultStoreId = null;
        _defaultCustomerId = null;

        await InstallStoresAsync();
        await InstallMeasuresAsync();
        await InstallTaxCategoriesAsync();
        await InstallLanguagesAsync();
        await InstallCurrenciesAsync();
        await InstallCountriesAndStatesAsync();
        await InstallShippingMethodsAsync();
        await InstallDeliveryDatesAsync();
        await InstallProductAvailabilityRangesAsync();
        await InstallEmailAccountsAsync();
        await InstallMessageTemplatesAsync();
        await InstallNewsLetterSubscriptionTypeAsync();
        await InstallTopicTemplatesAsync();
        await InstallSettingsAsync();
        await InstallCustomersAndUsersAsync();
        await InstallTopicsAsync();
        await InstallActivityLogTypesAsync();
        await InstallProductTemplatesAsync();
        await InstallCategoryTemplatesAsync();
        await InstallManufacturerTemplatesAsync();
        await InstallScheduleTasksAsync();
        await InstallReturnRequestReasonsAsync();
        await InstallReturnRequestActionsAsync();
        await InstallMenusAsync();

        if (!installationSettings.InstallSampleData)
            return;

        //save setting to install plugin sample data
        var setting = new Setting
        {
            Name = NopInstallationDefaults.InstallPluginSampleDataSettingName,
            Value = "true",
            StoreId = 0
        };

        await _dataProvider.InsertEntityAsync(setting);

        var sampleData = JsonConvert.DeserializeObject<SampleData.SampleData>(await _fileProvider.ReadAllTextAsync(_fileProvider.MapPath(NopInstallationDefaults.SampleDataPath), Encoding.UTF8));

        await InstallSampleCustomersAsync(sampleData.Customers);
        await InstallCheckoutAttributesAsync(sampleData.CheckoutAttributes);
        await InstallSpecificationAttributesAsync(sampleData.SpecificationAttributes);
        await InstallProductAttributesAsync(sampleData.ProductAttributes);
        await InstallCategoriesAsync(sampleData.Categories);
        await InstallManufacturersAsync(sampleData.Manufacturers);
        await InstallProductsAsync(sampleData.Products);
        await InstallForumsAsync(sampleData.ForumGroups);
        await InstallDiscountsAsync(sampleData.Discounts);
        await InstallBlogPostsAsync(sampleData.BlogPosts);
        await InstallNewsAsync(sampleData.NewsItems);
        await InstallPollsAsync(sampleData.Polls);
        await InstallWarehousesAsync(sampleData.Warehouses);
        await InstallVendorsAsync(sampleData.Vendors);
        await InstallAffiliatesAsync(sampleData.Affiliates);
        await InstallOrdersAsync(sampleData.Orders);
        await InstallActivityLogsAsync(sampleData.ActivityLogs);
        await InstallSearchTermsAsync(sampleData.SearchTerms);
    }

    #endregion
}