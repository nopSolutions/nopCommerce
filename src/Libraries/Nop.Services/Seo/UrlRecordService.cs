using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;

namespace Nop.Services.Seo
{
    /// <summary>
    /// Provides information about URL records
    /// </summary>
    public partial class UrlRecordService : IUrlRecordService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// {2} : language ID
        /// </remarks>
        private const string URLRECORD_ACTIVE_BY_ID_NAME_LANGUAGE_KEY = "Nop.urlrecord.active.id-name-language-{0}-{1}-{2}";
        /// <summary>
        /// Key for caching
        /// </summary>
        private const string URLRECORD_ALL_KEY = "Nop.urlrecord.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : slug
        /// </remarks>
        private const string URLRECORD_BY_SLUG_KEY = "Nop.urlrecord.active.slug-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string URLRECORD_PATTERN_KEY = "Nop.urlrecord.";

        #endregion

        #region Fields

        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly ICacheManager _cacheManager;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="urlRecordRepository">URL record repository</param>
        /// <param name="localizationSettings">Localization settings</param>
        public UrlRecordService(ICacheManager cacheManager,
            IRepository<UrlRecord> urlRecordRepository,
            LocalizationSettings localizationSettings)
        {
            this._cacheManager = cacheManager;
            this._urlRecordRepository = urlRecordRepository;
            this._localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        protected UrlRecordForCaching Map(UrlRecord record)
        {
            if (record == null)
                throw new ArgumentNullException("record");

            var urlRecordForCaching = new UrlRecordForCaching
            {
                Id = record.Id,
                EntityId = record.EntityId,
                EntityName = record.EntityName,
                Slug = record.Slug,
                IsActive = record.IsActive,
                LanguageId = record.LanguageId
            };
            return urlRecordForCaching;
        }

        /// <summary>
        /// Gets all cached URL records
        /// </summary>
        /// <returns>cached URL records</returns>
        protected virtual IList<UrlRecordForCaching> GetAllUrlRecordsCached()
        {
            //cache
            string key = string.Format(URLRECORD_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from ur in _urlRecordRepository.Table
                            select ur;
                var urlRecords = query.ToList();
                var list = new List<UrlRecordForCaching>();
                foreach (var ur in urlRecords)
                {
                    var urlRecordForCaching = Map(ur);
                    list.Add(urlRecordForCaching);
                }
                return list;
            });
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class UrlRecordForCaching
        {
            public int Id { get; set; }
            public int EntityId { get; set; }
            public string EntityName { get; set; }
            public string Slug { get; set; }
            public bool IsActive { get; set; }
            public int LanguageId { get; set; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an URL record
        /// </summary>
        /// <param name="urlRecord">URL record</param>
        public virtual void DeleteUrlRecord(UrlRecord urlRecord)
        {
            if (urlRecord == null)
                throw new ArgumentNullException("urlRecord");

            _urlRecordRepository.Delete(urlRecord);

            //cache
            _cacheManager.RemoveByPattern(URLRECORD_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes an URL records
        /// </summary>
        /// <param name="urlRecords">URL records</param>
        public virtual void DeleteUrlRecords(IList<UrlRecord> urlRecords)
        {
            if (urlRecords == null)
                throw new ArgumentNullException("urlRecords");

            _urlRecordRepository.Delete(urlRecords);

            //cache
            _cacheManager.RemoveByPattern(URLRECORD_PATTERN_KEY);
        }

        /// <summary>
        /// Gets an URL records
        /// </summary>
        /// <param name="urlRecordIds">URL record identifiers</param>
        /// <returns>URL record</returns>
        public virtual IList<UrlRecord> GetUrlRecordsByIds(int[] urlRecordIds)
        {
            var query = _urlRecordRepository.Table;

            return query.Where(p=>urlRecordIds.Contains(p.Id)).ToList();
        }

        /// <summary>
        /// Gets an URL record
        /// </summary>
        /// <param name="urlRecordId">URL record identifier</param>
        /// <returns>URL record</returns>
        public virtual UrlRecord GetUrlRecordById(int urlRecordId)
        {
            if (urlRecordId == 0)
                return null;

            return _urlRecordRepository.GetById(urlRecordId);
        }
        
        /// <summary>
        /// Inserts an URL record
        /// </summary>
        /// <param name="urlRecord">URL record</param>
        public virtual void InsertUrlRecord(UrlRecord urlRecord)
        {
            if (urlRecord == null)
                throw new ArgumentNullException("urlRecord");

            _urlRecordRepository.Insert(urlRecord);

            //cache
            _cacheManager.RemoveByPattern(URLRECORD_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the URL record
        /// </summary>
        /// <param name="urlRecord">URL record</param>
        public virtual void UpdateUrlRecord(UrlRecord urlRecord)
        {
            if (urlRecord == null)
                throw new ArgumentNullException("urlRecord");

            _urlRecordRepository.Update(urlRecord);

            //cache
            _cacheManager.RemoveByPattern(URLRECORD_PATTERN_KEY);
        }

        /// <summary>
        /// Find URL record
        /// </summary>
        /// <param name="slug">Slug</param>
        /// <returns>Found URL record</returns>
        public virtual UrlRecord GetBySlug(string slug)
        {
            if (String.IsNullOrEmpty(slug))
                return null;

            var query = from ur in _urlRecordRepository.Table
                        where ur.Slug == slug
                        //first, try to find an active record
                        orderby ur.IsActive descending, ur.Id
                        select ur;
            var urlRecord = query.FirstOrDefault();
            return urlRecord;
        }

        /// <summary>
        /// Find URL record (cached version).
        /// This method works absolutely the same way as "GetBySlug" one but caches the results.
        /// Hence, it's used only for performance optimization in public store
        /// </summary>
        /// <param name="slug">Slug</param>
        /// <returns>Found URL record</returns>
        public virtual UrlRecordForCaching GetBySlugCached(string slug)
        {
            if (String.IsNullOrEmpty(slug))
                return null;

            if (_localizationSettings.LoadAllUrlRecordsOnStartup)
            {
                //load all records (we know they are cached)
                var source = GetAllUrlRecordsCached();
                var query = from ur in source
                            where ur.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase)
                            //first, try to find an active record
                            orderby ur.IsActive descending, ur.Id
                            select ur;
                var urlRecordForCaching = query.FirstOrDefault();
                return urlRecordForCaching;
            }

            //gradual loading
            string key = string.Format(URLRECORD_BY_SLUG_KEY, slug);
            return _cacheManager.Get(key, () =>
            {
                var urlRecord = GetBySlug(slug);
                if (urlRecord == null)
                    return null;

                var urlRecordForCaching = Map(urlRecord);
                return urlRecordForCaching;
            });
        }

        /// <summary>
        /// Gets all URL records
        /// </summary>
        /// <param name="slug">Slug</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>URL records</returns>
        public virtual IPagedList<UrlRecord> GetAllUrlRecords(string slug = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _urlRecordRepository.Table;
            if (!String.IsNullOrWhiteSpace(slug))
                query = query.Where(ur => ur.Slug.Contains(slug));
            query = query.OrderBy(ur => ur.Slug);

            var urlRecords = new PagedList<UrlRecord>(query, pageIndex, pageSize);
            return urlRecords;
        }

        /// <summary>
        /// Find slug
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="entityName">Entity name</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Found slug</returns>
        public virtual string GetActiveSlug(int entityId, string entityName, int languageId)
        {
            if (_localizationSettings.LoadAllUrlRecordsOnStartup)
            {
                string key = string.Format(URLRECORD_ACTIVE_BY_ID_NAME_LANGUAGE_KEY, entityId, entityName, languageId);
                return _cacheManager.Get(key, () =>
                {
                    //load all records (we know they are cached)
                    var source = GetAllUrlRecordsCached();
                    var query = from ur in source
                                where ur.EntityId == entityId &&
                                ur.EntityName == entityName &&
                                ur.LanguageId == languageId &&
                                ur.IsActive
                                orderby ur.Id descending
                                select ur.Slug;
                    var slug = query.FirstOrDefault();
                    //little hack here. nulls aren't cacheable so set it to ""
                    if (slug == null)
                        slug = "";
                    return slug;
                });
            }
            else
            {
                //gradual loading
                string key = string.Format(URLRECORD_ACTIVE_BY_ID_NAME_LANGUAGE_KEY, entityId, entityName, languageId);
                return _cacheManager.Get(key, () =>
                {
                    var source = _urlRecordRepository.Table;
                    var query = from ur in source
                                where ur.EntityId == entityId &&
                                ur.EntityName == entityName &&
                                ur.LanguageId == languageId &&
                                ur.IsActive
                                orderby ur.Id descending
                                select ur.Slug;
                    var slug = query.FirstOrDefault();
                    //little hack here. nulls aren't cacheable so set it to ""
                    if (slug == null)
                        slug = "";
                    return slug;
                });
            }
        }

        /// <summary>
        /// Save slug
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="slug">Slug</param>
        /// <param name="languageId">Language ID</param>
        public virtual void SaveSlug<T>(T entity, string slug, int languageId) where T : BaseEntity, ISlugSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            int entityId = entity.Id;
            string entityName = typeof(T).Name;

            var query = from ur in _urlRecordRepository.Table
                        where ur.EntityId == entityId &&
                        ur.EntityName == entityName &&
                        ur.LanguageId == languageId
                        orderby ur.Id descending 
                        select ur;
            var allUrlRecords = query.ToList();
            var activeUrlRecord = allUrlRecords.FirstOrDefault(x => x.IsActive);

            if (activeUrlRecord == null && !string.IsNullOrWhiteSpace(slug))
            {
                //find in non-active records with the specified slug
                var nonActiveRecordWithSpecifiedSlug = allUrlRecords
                    .FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && !x.IsActive);
                if (nonActiveRecordWithSpecifiedSlug != null)
                {
                    //mark non-active record as active
                    nonActiveRecordWithSpecifiedSlug.IsActive = true;
                    UpdateUrlRecord(nonActiveRecordWithSpecifiedSlug);
                }
                else
                {
                    //new record
                    var urlRecord = new UrlRecord
                    {
                        EntityId = entityId,
                        EntityName = entityName,
                        Slug = slug,
                        LanguageId = languageId,
                        IsActive = true,
                    };
                    InsertUrlRecord(urlRecord);
                }
            }

            if (activeUrlRecord != null && string.IsNullOrWhiteSpace(slug))
            {
                //disable the previous active URL record
                activeUrlRecord.IsActive = false;
                UpdateUrlRecord(activeUrlRecord);
            }

            if (activeUrlRecord != null && !string.IsNullOrWhiteSpace(slug))
            {
                //it should not be the same slug as in active URL record
                if (!activeUrlRecord.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                {
                    //find in non-active records with the specified slug
                    var nonActiveRecordWithSpecifiedSlug = allUrlRecords
                        .FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && !x.IsActive);
                    if (nonActiveRecordWithSpecifiedSlug != null)
                    {
                        //mark non-active record as active
                        nonActiveRecordWithSpecifiedSlug.IsActive = true;
                        UpdateUrlRecord(nonActiveRecordWithSpecifiedSlug);

                        //disable the previous active URL record
                        activeUrlRecord.IsActive = false;
                        UpdateUrlRecord(activeUrlRecord);
                    }
                    else
                    {
                        //insert new record
                        //we do not update the existing record because we should track all previously entered slugs
                        //to ensure that URLs will work fine
                        var urlRecord = new UrlRecord
                        {
                            EntityId = entityId,
                            EntityName = entityName,
                            Slug = slug,
                            LanguageId = languageId,
                            IsActive = true,
                        };
                        InsertUrlRecord(urlRecord);

                        //disable the previous active URL record
                        activeUrlRecord.IsActive = false;
                        UpdateUrlRecord(activeUrlRecord);
                    }

                }
            }
        }

        #endregion
    }
}