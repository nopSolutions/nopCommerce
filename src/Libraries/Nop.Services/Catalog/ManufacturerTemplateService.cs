using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Manufacturer template service
    /// </summary>
    public partial class ManufacturerTemplateService : IManufacturerTemplateService
    {
        #region Constants
        private const string MANUFACTURERTEMPLATES_BY_ID_KEY = "Nop.manufacturertemplate.id-{0}";
        private const string MANUFACTURERTEMPLATES_ALL_KEY = "Nop.manufacturertemplate.all";
        private const string MANUFACTURERTEMPLATES_PATTERN_KEY = "Nop.manufacturertemplate.";

        #endregion

        #region Fields

        private readonly IRepository<ManufacturerTemplate> _manufacturerTemplateRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="manufacturerTemplateRepository">Manufacturer template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ManufacturerTemplateService(ICacheManager cacheManager,
            IRepository<ManufacturerTemplate> manufacturerTemplateRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _manufacturerTemplateRepository = manufacturerTemplateRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public virtual void DeleteManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            if (manufacturerTemplate == null)
                throw new ArgumentNullException("manufacturerTemplate");

            _manufacturerTemplateRepository.Delete(manufacturerTemplate);

            _cacheManager.RemoveByPattern(MANUFACTURERTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(manufacturerTemplate);
        }

        /// <summary>
        /// Gets all manufacturer templates
        /// </summary>
        /// <returns>Manufacturer templates</returns>
        public virtual IList<ManufacturerTemplate> GetAllManufacturerTemplates()
        {
            string key = MANUFACTURERTEMPLATES_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from pt in _manufacturerTemplateRepository.Table
                            orderby pt.DisplayOrder
                            select pt;

                var templates = query.ToList();
                return templates;
            });
        }
 
        /// <summary>
        /// Gets a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        /// <returns>Manufacturer template</returns>
        public virtual ManufacturerTemplate GetManufacturerTemplateById(int manufacturerTemplateId)
        {
            if (manufacturerTemplateId == 0)
                return null;

            string key = string.Format(MANUFACTURERTEMPLATES_BY_ID_KEY, manufacturerTemplateId);
            return _cacheManager.Get(key, () =>
            {
                var template = _manufacturerTemplateRepository.GetById(manufacturerTemplateId);
                return template;
            });
        }

        /// <summary>
        /// Inserts manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public virtual void InsertManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            if (manufacturerTemplate == null)
                throw new ArgumentNullException("manufacturerTemplate");

            _manufacturerTemplateRepository.Insert(manufacturerTemplate);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(manufacturerTemplate);
        }

        /// <summary>
        /// Updates the manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public virtual void UpdateManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            if (manufacturerTemplate == null)
                throw new ArgumentNullException("manufacturerTemplate");

            _manufacturerTemplateRepository.Update(manufacturerTemplate);

            //cache
            _cacheManager.RemoveByPattern(MANUFACTURERTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(manufacturerTemplate);
        }
        
        #endregion
    }
}
