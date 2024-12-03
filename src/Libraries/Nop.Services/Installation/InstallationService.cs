using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Seo;

namespace Nop.Services.Installation;

/// <summary>
/// Installation service
/// </summary>
public partial class InstallationService : IInstallationService
{
    #region Fields

    protected readonly INopDataProvider _dataProvider;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IWebHelper _webHelper;

    protected string _defaultCustomerEmail;

    #endregion

    #region Ctor

    public InstallationService(INopDataProvider dataProvider,
        INopFileProvider fileProvider,
        IWebHelper webHelper)
    {
        _dataProvider = dataProvider;
        _fileProvider = fileProvider;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

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
        if (string.IsNullOrEmpty(_defaultCustomerEmail) || !_defaultCustomerEmail.Equals(defaultUserEmail))
        {
            _defaultCustomerEmail = defaultUserEmail;
            _defaultCustomerId = null;
        }

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
        await InstallCustomersAndUsersAsync(defaultUserPassword);
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
        if (string.IsNullOrEmpty(_defaultCustomerEmail) || !_defaultCustomerEmail.Equals(defaultUserEmail))
        {
            _defaultCustomerEmail = defaultUserEmail;
            _defaultCustomerId = null;
        }

        await InstallSampleCustomersAsync();
        await InstallCheckoutAttributesAsync();
        await InstallSpecificationAttributesAsync();
        await InstallProductAttributesAsync();
        await InstallCategoriesAsync();
        await InstallManufacturersAsync();
        await InstallProductsAsync();
        await InstallForumsAsync();
        await InstallDiscountsAsync();
        await InstallBlogPostsAsync();
        await InstallNewsAsync();
        await InstallPollsAsync();
        await InstallWarehousesAsync();
        await InstallVendorsAsync();
        await InstallAffiliatesAsync();
        await InstallOrdersAsync();
        await InstallActivityLogsAsync();
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