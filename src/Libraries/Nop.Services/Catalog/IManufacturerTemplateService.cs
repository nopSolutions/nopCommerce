using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Manufacturer template service interface
    /// </summary>
    public partial interface IManufacturerTemplateService
    {
        /// <summary>
        /// Delete manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        void DeleteManufacturerTemplate(ManufacturerTemplate manufacturerTemplate);

        /// <summary>
        /// Gets all manufacturer templates
        /// </summary>
        /// <returns>Manufacturer templates</returns>
        IList<ManufacturerTemplate> GetAllManufacturerTemplates();

        /// <summary>
        /// Gets a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        /// <returns>Manufacturer template</returns>
        ManufacturerTemplate GetManufacturerTemplateById(int manufacturerTemplateId);

        /// <summary>
        /// Inserts manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        void InsertManufacturerTemplate(ManufacturerTemplate manufacturerTemplate);

        /// <summary>
        /// Updates the manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        void UpdateManufacturerTemplate(ManufacturerTemplate manufacturerTemplate);
    }
}
