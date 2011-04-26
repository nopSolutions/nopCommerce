
using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Media
{
    /// <summary>
    /// Download service interface
    /// </summary>
    public partial interface IDownloadService
    {
        /// <summary>
        /// Gets a download
        /// </summary>
        /// <param name="downloadId">Download identifier</param>
        /// <returns>Download</returns>
        Download GetDownloadById(int downloadId);

        /// <summary>
        /// Deletes a download
        /// </summary>
        /// <param name="download">Download</param>
        void DeleteDownload(Download download);

        /// <summary>
        /// Inserts a download
        /// </summary>
        /// <param name="download">Download</param>
        void InsertDownload(Download download);

        /// <summary>
        /// Updates the download
        /// </summary>
        /// <param name="download">Download</param>
        void UpdateDownload(Download download);
    }
}
