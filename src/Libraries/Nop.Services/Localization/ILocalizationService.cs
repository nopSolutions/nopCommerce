using System.Linq.Expressions;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Services.Plugins;

namespace Nop.Services.Localization;

/// <summary>
/// Localization manager interface
/// </summary>
public partial interface ILocalizationService
{
    /// <summary>
    /// Deletes a locale string resource
    /// </summary>
    /// <param name="localeStringResource">Locale string resource</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteLocaleStringResourceAsync(LocaleStringResource localeStringResource);

    /// <summary>
    /// Deletes a locale string resource
    /// </summary>
    /// <param name="localeStringResource">Locale string resource</param>
    void DeleteLocaleStringResource(LocaleStringResource localeStringResource);

    /// <summary>
    /// Gets a locale string resource
    /// </summary>
    /// <param name="localeStringResourceId">Locale string resource identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the locale string resource
    /// </returns>
    Task<LocaleStringResource> GetLocaleStringResourceByIdAsync(int localeStringResourceId);

    /// <summary>
    /// Gets a locale string resource
    /// </summary>
    /// <param name="resourceName">A string representing a resource name</param>
    /// <param name="languageId">Language identifier</param>
    /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the locale string resource
    /// </returns>
    Task<LocaleStringResource> GetLocaleStringResourceByNameAsync(string resourceName, int languageId,
        bool logIfNotFound = true);

    /// <summary>
    /// Gets a locale string resource
    /// </summary>
    /// <param name="resourceName">A string representing a resource name</param>
    /// <param name="languageId">Language identifier</param>
    /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
    /// <returns>
    /// The locale string resource
    /// </returns>
    LocaleStringResource GetLocaleStringResourceByName(string resourceName, int languageId,
        bool logIfNotFound = true);

    /// <summary>
    /// Inserts a locale string resource
    /// </summary>
    /// <param name="localeStringResource">Locale string resource</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertLocaleStringResourceAsync(LocaleStringResource localeStringResource);

    /// <summary>
    /// Updates the locale string resource
    /// </summary>
    /// <param name="localeStringResource">Locale string resource</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateLocaleStringResourceAsync(LocaleStringResource localeStringResource);

    /// <summary>
    /// Updates the locale string resource
    /// </summary>
    /// <param name="localeStringResource">Locale string resource</param>
    void UpdateLocaleStringResource(LocaleStringResource localeStringResource);

    /// <summary>
    /// Gets all locale string resources by language identifier
    /// </summary>
    /// <param name="languageId">Language identifier</param>
    /// <param name="loadPublicLocales">A value indicating whether to load data for the public store only (if "false", then for admin area only. If null, then load all locales. We use it for performance optimization of the site startup</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the locale string resources
    /// </returns>
    Task<Dictionary<string, KeyValuePair<int, string>>> GetAllResourceValuesAsync(int languageId, bool? loadPublicLocales);

    /// <summary>
    /// Gets a resource string based on the specified ResourceKey property.
    /// </summary>
    /// <param name="resourceKey">A string representing a ResourceKey.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a string representing the requested resource string.
    /// </returns>
    Task<string> GetResourceAsync(string resourceKey);

    /// <summary>
    /// Gets a resource string based on the specified ResourceKey property.
    /// </summary>
    /// <param name="resourceKey">A string representing a ResourceKey.</param>
    /// <param name="languageId">Language identifier</param>
    /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="returnEmptyIfNotFound">A value indicating whether an empty string will be returned if a resource is not found and default value is set to empty string</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a string representing the requested resource string.
    /// </returns>
    Task<string> GetResourceAsync(string resourceKey, int languageId,
        bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false);

    /// <summary>
    /// Export language resources to XML
    /// </summary>
    /// <param name="language">Language</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result in XML format
    /// </returns>
    Task<string> ExportResourcesToXmlAsync(Language language);

    /// <summary>
    /// Import language resources from XML file
    /// </summary>
    /// <param name="language">Language</param>
    /// <param name="xmlStreamReader">Stream reader of XML file</param>
    /// <param name="updateExistingResources">A value indicating whether to update existing resources</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task ImportResourcesFromXmlAsync(Language language, StreamReader xmlStreamReader, bool updateExistingResources = true);

    /// <summary>
    /// Get localized property of an entity
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="languageId">Language identifier; pass null to use the current working language; pass 0 to get standard language value</param>
    /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
    /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localized property
    /// </returns>
    Task<TPropType> GetLocalizedAsync<TEntity, TPropType>(TEntity entity, Expression<Func<TEntity, TPropType>> keySelector,
        int? languageId = null, bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
        where TEntity : BaseEntity, ILocalizedEntity;

    /// <summary>
    /// Get localized property of setting
    /// </summary>
    /// <typeparam name="TSettings">Settings type</typeparam>
    /// <param name="settings">Settings</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="languageId">Language identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
    /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localized property
    /// </returns>
    Task<string> GetLocalizedSettingAsync<TSettings>(TSettings settings, Expression<Func<TSettings, string>> keySelector,
        int languageId, int storeId, bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
        where TSettings : ISettings, new();

    /// <summary>
    /// Save localized property of setting
    /// </summary>
    /// <typeparam name="TSettings">Settings type</typeparam>
    /// <param name="settings">Settings</param>
    /// <param name="keySelector">Key selector</param>
    /// <param name="languageId">Language identifier</param>
    /// <param name="value">Localized value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localized property
    /// </returns>
    Task SaveLocalizedSettingAsync<TSettings>(TSettings settings, Expression<Func<TSettings, string>> keySelector,
        int languageId, string value) where TSettings : ISettings, new();

    /// <summary>
    /// Get localized value of enum
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <param name="enumValue">Enum value</param>
    /// <param name="languageId">Language identifier; pass null to use the current working language</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localized value
    /// </returns>
    Task<string> GetLocalizedEnumAsync<TEnum>(TEnum enumValue, int? languageId = null) where TEnum : struct;

    /// <summary>
    /// Get localized value of enum
    /// We don't have UI to manage permission localizable name. That's why we're using this method
    /// </summary>
    /// <param name="permissionRecord">Permission record</param>
    /// <param name="languageId">Language identifier; pass null to use the current working language</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localized value
    /// </returns>
    Task<string> GetLocalizedPermissionNameAsync(PermissionRecord permissionRecord, int? languageId = null);

    /// <summary>
    /// Save localized name of a permission
    /// </summary>
    /// <param name="permissionRecord">Permission record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SaveLocalizedPermissionNameAsync(PermissionRecord permissionRecord);

    /// <summary>
    /// Delete a localized name of a permission
    /// </summary>
    /// <param name="permissionRecord">Permission record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteLocalizedPermissionNameAsync(PermissionRecord permissionRecord);

    /// <summary>
    /// Add a locale resource (if new) or update an existing one
    /// </summary>
    /// <param name="resourceName">Resource name</param>
    /// <param name="resourceValue">Resource value</param>
    /// <param name="languageCulture">Language culture code. If null or empty, then a resource will be added for all languages</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddOrUpdateLocaleResourceAsync(string resourceName, string resourceValue, string languageCulture = null);

    /// <summary>
    /// Add locale resources
    /// </summary>
    /// <param name="resources">Resource name-value pairs</param>
    /// <param name="languageId">Language identifier; pass null to add the passed resources for all languages</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task AddOrUpdateLocaleResourceAsync(IDictionary<string, string> resources, int? languageId = null);

    /// <summary>
    /// Add locale resources
    /// </summary>
    /// <param name="resources">Resource name-value pairs</param>
    /// <param name="languageId">Language identifier; pass null to add the passed resources for all languages</param>
    void AddOrUpdateLocaleResource(IDictionary<string, string> resources, int? languageId = null);

    /// <summary>
    /// Delete a locale resource
    /// </summary>
    /// <param name="resourceName">Resource name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteLocaleResourceAsync(string resourceName);

    /// <summary>
    /// Delete locale resources
    /// </summary>
    /// <param name="resourceNames">Resource names</param>
    /// <param name="languageId">Language identifier; pass null to delete the passed resources from all languages</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteLocaleResourcesAsync(IList<string> resourceNames, int? languageId = null);

    /// <summary>
    /// Delete locale resources
    /// </summary>
    /// <param name="resourceNames">Resource names</param>
    /// <param name="languageId">Language identifier; pass null to delete the passed resources from all languages</param>
    void DeleteLocaleResources(IList<string> resourceNames, int? languageId = null);

    /// <summary>
    /// Delete locale resources by the passed name prefix
    /// </summary>
    /// <param name="resourceNamePrefix">Resource name prefix</param>
    /// <param name="languageId">Language identifier; pass null to delete resources by prefix from all languages</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteLocaleResourcesAsync(string resourceNamePrefix, int? languageId = null);

    /// <summary>
    /// Get localized friendly name of a plugin
    /// </summary>
    /// <typeparam name="TPlugin">Plugin type</typeparam>
    /// <param name="plugin">Plugin</param>
    /// <param name="languageId">Language identifier</param>
    /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localized value
    /// </returns>
    Task<string> GetLocalizedFriendlyNameAsync<TPlugin>(TPlugin plugin, int languageId, bool returnDefaultValue = true)
        where TPlugin : IPlugin;

    /// <summary>
    /// Save localized friendly name of a plugin
    /// </summary>
    /// <typeparam name="TPlugin">Plugin</typeparam>
    /// <param name="plugin">Plugin</param>
    /// <param name="languageId">Language identifier</param>
    /// <param name="localizedFriendlyName">Localized friendly name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SaveLocalizedFriendlyNameAsync<TPlugin>(TPlugin plugin, int languageId, string localizedFriendlyName)
        where TPlugin : IPlugin;
}