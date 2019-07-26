using System.Collections.Generic;
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
        /// <returns>List of uploaded items descriptor</returns>
        IList<IDescriptor> UploadPluginsAndThemes(IFormFile archivefile);

        /// <summary>
        /// Upload favicon and app icons
        /// </summary>
        /// <param name="archivefile">Archive file</param>
        void UploadIconsArchive(IFormFile archivefile);

    }
}