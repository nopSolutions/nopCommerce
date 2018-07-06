using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Seo;

namespace Nop.Services.Seo
{
    /// <summary>
    /// Provides information about URL records
    /// </summary>
    public partial interface IUrlRecordService
    {
        /// <summary>
        /// Deletes an URL record
        /// </summary>
        /// <param name="urlRecord">URL record</param>
        void DeleteUrlRecord(UrlRecord urlRecord);

        /// <summary>
        /// Deletes an URL records
        /// </summary>
        /// <param name="urlRecords">URL records</param>
        void DeleteUrlRecords(IList<UrlRecord> urlRecords);

        /// <summary>
        /// Gets an URL record
        /// </summary>
        /// <param name="urlRecordId">URL record identifier</param>
        /// <returns>URL record</returns>
        UrlRecord GetUrlRecordById(int urlRecordId);

        /// <summary>
        /// Gets an URL records
        /// </summary>
        /// <param name="urlRecordIds">URL record identifiers</param>
        /// <returns>URL record</returns>
        IList<UrlRecord> GetUrlRecordsByIds(int[] urlRecordIds);

        /// <summary>
        /// Inserts an URL record
        /// </summary>
        /// <param name="urlRecord">URL record</param>
        void InsertUrlRecord(UrlRecord urlRecord);

        /// <summary>
        /// Updates the URL record
        /// </summary>
        /// <param name="urlRecord">URL record</param>
        void UpdateUrlRecord(UrlRecord urlRecord);

        /// <summary>
        /// Find URL record
        /// </summary>
        /// <param name="slug">Slug</param>
        /// <returns>Found URL record</returns>
        UrlRecord GetBySlug(string slug);

        /// <summary>
        /// Find URL record (cached version).
        /// This method works absolutely the same way as "GetBySlug" one but caches the results.
        /// Hence, it's used only for performance optimization in public store
        /// </summary>
        /// <param name="slug">Slug</param>
        /// <returns>Found URL record</returns>
        UrlRecordService.UrlRecordForCaching GetBySlugCached(string slug);

        /// <summary>
        /// Gets all URL records
        /// </summary>
        /// <param name="slug">Slug</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>URL records</returns>
        IPagedList<UrlRecord> GetAllUrlRecords(string slug = "", int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Find slug
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="entityName">Entity name</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Found slug</returns>
        string GetActiveSlug(int entityId, string entityName, int languageId);

        /// <summary>
        /// Save slug
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="slug">Slug</param>
        /// <param name="languageId">Language ID</param>
        void SaveSlug<T>(T entity, string slug, int languageId) where T : BaseEntity, ISlugSupported;

        /// <summary>
        ///  Get search engine friendly name (slug)
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="languageId">Language identifier; pass null to use the current language</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if language specified one is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Search engine  name (slug)</returns>
        string GetSeName<T>(T entity, int? languageId = null, bool returnDefaultValue = true,
            bool ensureTwoPublishedLanguages = true) where T : BaseEntity, ISlugSupported;

        /// <summary>
        /// Get search engine friendly name (slug)
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="entityName">Entity name</param>
        /// <param name="languageId">Language identifier; pass null to use the current language</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if language specified one is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Search engine  name (slug)</returns>
        string GetSeName(int entityId, string entityName, int? languageId = null,
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true);

        /// <summary>
        /// Get SE name
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="convertNonWesternChars">A value indicating whether non western chars should be converted</param>
        /// <param name="allowUnicodeCharsInUrls">A value indicating whether Unicode chars are allowed</param>
        /// <returns>Result</returns>
        string GetSeName(string name, bool convertNonWesternChars, bool allowUnicodeCharsInUrls);

        /// <summary>
        /// Validate search engine name
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="seName">Search engine name to validate</param>
        /// <param name="name">User-friendly name used to generate sename</param>
        /// <param name="ensureNotEmpty">Ensure that sename is not empty</param>
        /// <returns>Valid sename</returns>
        string ValidateSeName<T>(T entity, string seName, string name, bool ensureNotEmpty) where T : BaseEntity, ISlugSupported;

        /// <summary>
        /// Validate search engine name
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="entityName">Entity name</param>
        /// <param name="seName">Search engine name to validate</param>
        /// <param name="name">User-friendly name used to generate sename</param>
        /// <param name="ensureNotEmpty">Ensure that sename is not empty</param>
        /// <returns>Valid sename</returns>
        string ValidateSeName(int entityId, string entityName, string seName, string name, bool ensureNotEmpty);
    }
}