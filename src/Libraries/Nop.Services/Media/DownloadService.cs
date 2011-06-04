
using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;

using Nop.Services.Configuration;

namespace Nop.Services.Media
{
    /// <summary>
    /// Download service
    /// </summary>
    public partial class DownloadService : IDownloadService
    {
        #region Fields

        private readonly IRepository<Download> _downloadRepository;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="downloadRepository">Download repository</param>
        /// <param name="webHelper">Web helper</param>
        public DownloadService(IRepository<Download> downloadRepository)
        {
            this._downloadRepository = downloadRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a download
        /// </summary>
        /// <param name="downloadId">Download identifier</param>
        /// <returns>Download</returns>
        public virtual Download GetDownloadById(int downloadId)
        {
            if (downloadId == 0)
                return null;
            
            var download = _downloadRepository.GetById(downloadId);
            return download;
        }

        /// <summary>
        /// Deletes a download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void DeleteDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Delete(download);
        }

        /// <summary>
        /// Inserts a download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void InsertDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Insert(download);
        }

        /// <summary>
        /// Updates the download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void UpdateDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Update(download);
        }
        #endregion
    }
}
