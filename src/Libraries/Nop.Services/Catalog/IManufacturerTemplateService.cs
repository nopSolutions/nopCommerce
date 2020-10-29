using System.Collections.Generic;
using System.Threading.Tasks;
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
        Task DeleteManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);

        /// <summary>
        /// Gets all manufacturer templates
        /// </summary>
        /// <returns>Manufacturer templates</returns>
        Task<IList<ManufacturerTemplate>> GetAllManufacturerTemplatesAsync();

        /// <summary>
        /// Gets a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        /// <returns>Manufacturer template</returns>
        Task<ManufacturerTemplate> GetManufacturerTemplateByIdAsync(int manufacturerTemplateId);

        /// <summary>
        /// Inserts manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        Task InsertManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);

        /// <summary>
        /// Updates the manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        Task UpdateManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);
    }
}
