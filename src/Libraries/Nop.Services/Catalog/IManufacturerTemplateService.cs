<<<<<<< HEAD
﻿using System.Collections.Generic;
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
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);

        /// <summary>
        /// Gets all manufacturer templates
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer templates
        /// </returns>
        Task<IList<ManufacturerTemplate>> GetAllManufacturerTemplatesAsync();

        /// <summary>
        /// Gets a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer template
        /// </returns>
        Task<ManufacturerTemplate> GetManufacturerTemplateByIdAsync(int manufacturerTemplateId);

        /// <summary>
        /// Inserts manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);

        /// <summary>
        /// Updates the manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);
    }
}
=======
﻿using System.Collections.Generic;
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
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);

        /// <summary>
        /// Gets all manufacturer templates
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer templates
        /// </returns>
        Task<IList<ManufacturerTemplate>> GetAllManufacturerTemplatesAsync();

        /// <summary>
        /// Gets a manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer template
        /// </returns>
        Task<ManufacturerTemplate> GetManufacturerTemplateByIdAsync(int manufacturerTemplateId);

        /// <summary>
        /// Inserts manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);

        /// <summary>
        /// Updates the manufacturer template
        /// </summary>
        /// <param name="manufacturerTemplate">Manufacturer template</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate);
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
