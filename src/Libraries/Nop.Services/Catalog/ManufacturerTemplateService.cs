using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Manufacturer template service
    /// </summary>
    public partial class ManufacturerTemplateService : IManufacturerTemplateService
    {
        #region Fields

        private readonly IRepository<ManufacturerTemplate> _manufacturerTemplateRepository;

        #endregion

        #region Ctor

        public ManufacturerTemplateService(IRepository<ManufacturerTemplate> manufacturerTemplateRepository)
        {
            _manufacturerTemplateRepository = manufacturerTemplateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public virtual async Task DeleteManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            await _manufacturerTemplateRepository.Delete(manufacturerTemplate);
        }

        /// <summary>
        /// Gets all manufacturer templates
        /// </summary>
        /// <returns>Manufacturer templates</returns>
        public virtual async Task<IList<ManufacturerTemplate>> GetAllManufacturerTemplates()
        {
            var templates = await _manufacturerTemplateRepository.GetAll(query =>
            {
                return from pt in query
                    orderby pt.DisplayOrder, pt.Id
                    select pt;
            }, cache => default);

            return templates;
        }

        /// <summary>
        /// Gets a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        /// <returns>Manufacturer template</returns>
        public virtual async Task<ManufacturerTemplate> GetManufacturerTemplateById(int manufacturerTemplateId)
        {
            return await _manufacturerTemplateRepository.GetById(manufacturerTemplateId, cache => default);
        }

        /// <summary>
        /// Inserts manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public virtual async Task InsertManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            await _manufacturerTemplateRepository.Insert(manufacturerTemplate);
        }

        /// <summary>
        /// Updates the manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        public virtual async Task UpdateManufacturerTemplate(ManufacturerTemplate manufacturerTemplate)
        {
            await _manufacturerTemplateRepository.Update(manufacturerTemplate);
        }

        #endregion
    }
}