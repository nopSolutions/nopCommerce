using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace Nop.Services.Plugins
{
    /// <summary>
    /// Represents a service for uploading application extensions (plugins or themes) and favicon and app icons
    /// </summary>
    public partial interface IUploadService
    {
        /// <summary>
        /// Upload plugins and/or themes
        /// </summary>
        /// <param name="archivefile">Archive file</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of uploaded items descriptor
        /// </returns>
        Task<IList<IDescriptor>> UploadPluginsAndThemesAsync(IFormFile archivefile);

        /// <summary>
        /// Upload favicon and app icons
        /// </summary>
        /// <param name="archivefile">Archive file which contains a set of special icons for different OS and devices</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UploadIconsArchiveAsync(IFormFile archivefile);

        /// <summary>
        /// Upload single favicon
        /// </summary>
        /// <param name="favicon">Favicon</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UploadFaviconAsync(IFormFile favicon);

        /// <summary>
        /// Upload locale pattern for current culture
        /// </summary>
        /// <param name="cultureInfo">CultureInfo</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UploadLocalePatternAsync(CultureInfo cultureInfo = null);
    }
}