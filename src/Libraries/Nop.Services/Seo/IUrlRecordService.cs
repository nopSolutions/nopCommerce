using Nop.Core;
using Nop.Core.Domain.Seo;

namespace Nop.Services.Seo;

/// <summary>
/// Provides information about URL records
/// </summary>
public partial interface IUrlRecordService
{
    /// <summary>
    /// Deletes an URL records
    /// </summary>
    /// <param name="urlRecords">URL records</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteUrlRecordsAsync(IList<UrlRecord> urlRecords);

    /// <summary>
    /// Gets an URL records
    /// </summary>
    /// <param name="urlRecordIds">URL record identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uRL record
    /// </returns>
    Task<IList<UrlRecord>> GetUrlRecordsByIdsAsync(int[] urlRecordIds);

    /// <summary>
    /// Inserts an URL record
    /// </summary>
    /// <param name="urlRecord">URL record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertUrlRecordAsync(UrlRecord urlRecord);

    /// <summary>
    /// Update an URL record
    /// </summary>
    /// <param name="urlRecord">URL record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateUrlRecordAsync(UrlRecord urlRecord);

    /// <summary>
    /// Find URL record
    /// </summary>
    /// <param name="slug">Slug</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found URL record
    /// </returns>
    Task<UrlRecord> GetBySlugAsync(string slug);

    /// <summary>
    /// Gets all URL records
    /// </summary>
    /// <param name="slug">Slug</param>
    /// <param name="languageId">Language ID; "null" to load records with any language; "0" to load records with standard language only; otherwise to load records with specify language ID only</param>
    /// <param name="isActive">A value indicating whether to get active records; "null" to load all records; "false" to load only inactive records; "true" to load only active records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uRL records
    /// </returns>
    Task<IPagedList<UrlRecord>> GetAllUrlRecordsAsync(string slug = "", int? languageId = null, bool? isActive = null, int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Find slug
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="entityName">Entity name</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found slug
    /// </returns>
    Task<string> GetActiveSlugAsync(int entityId, string entityName, int languageId);

    /// <summary>
    /// Save slug
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="slug">Slug</param>
    /// <param name="languageId">Language ID</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SaveSlugAsync<T>(T entity, string slug, int languageId) where T : BaseEntity, ISlugSupported;

    /// <summary>
    ///  Get search engine friendly name (slug)
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="languageId">Language identifier; pass null to use the current language</param>
    /// <param name="returnDefaultValue">A value indicating whether to return default value (if language specified one is not found)</param>
    /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search engine  name (slug)
    /// </returns>
    Task<string> GetSeNameAsync<T>(T entity, int? languageId = null, bool returnDefaultValue = true,
        bool ensureTwoPublishedLanguages = true) where T : BaseEntity, ISlugSupported;

    /// <summary>
    /// Get search engine friendly name (slug)
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="entityName">Entity name</param>
    /// <param name="languageId">Language identifier; pass null to use the current language</param>
    /// <param name="returnDefaultValue">A value indicating whether to return default value (if language specified one is not found)</param>
    /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search engine  name (slug)
    /// </returns>
    Task<string> GetSeNameAsync(int entityId, string entityName, int? languageId = null,
        bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true);

    /// <summary>
    /// Get SE name
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="convertNonWesternChars">A value indicating whether non western chars should be converted</param>
    /// <param name="allowUnicodeCharsInUrls">A value indicating whether Unicode chars are allowed</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<string> GetSeNameAsync(string name, bool convertNonWesternChars, bool allowUnicodeCharsInUrls);

    /// <summary>
    /// Validate search engine name
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="seName">Search engine name to validate</param>
    /// <param name="name">User-friendly name used to generate seName</param>
    /// <param name="ensureNotEmpty">Ensure that seName is not empty</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the valid seName
    /// </returns>
    Task<string> ValidateSeNameAsync<T>(T entity, string seName, string name, bool ensureNotEmpty) where T : BaseEntity, ISlugSupported;

    /// <summary>
    /// Validate search engine name
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="entityName">Entity name</param>
    /// <param name="seName">Search engine name to validate</param>
    /// <param name="name">User-friendly name used to generate seName</param>
    /// <param name="ensureNotEmpty">Ensure that seName is not empty</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the valid seName
    /// </returns>
    Task<string> ValidateSeNameAsync(int entityId, string entityName, string seName, string name, bool ensureNotEmpty);
}