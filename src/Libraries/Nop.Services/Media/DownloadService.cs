//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data;
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
        private readonly IWebHelper _webHelper;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="downloadRepository">Download repository</param>
        /// <param name="webHelper">Web helper</param>
        public DownloadService(IRepository<Download> downloadRepository,
            IWebHelper webHelper)
        {
            this._downloadRepository = downloadRepository;
            this._webHelper = webHelper;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets a sample download url for a product variant
        /// </summary>
        /// <param name="productVariant">Product variant instance</param>
        /// <returns>Download url</returns>
        public string GetSampleDownloadUrl(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            string url = string.Empty;
            if (productVariant.IsDownload && productVariant.HasSampleDownload)
            {
                //TODO Implement GetDownload.ashx
                url = string.Format("{0}GetDownload.ashx?SampleDownloadProductVariantID={1}", _webHelper.GetStoreLocation(), productVariant.Id);
            }
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets a download
        /// </summary>
        /// <param name="downloadId">Download identifier</param>
        /// <returns>Download</returns>
        public Download GetDownloadById(int downloadId)
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
        public void DeleteDownload(Download download)
        {
            if (download == null)
                return;

            _downloadRepository.Delete(download);
        }

        /// <summary>
        /// Inserts a download
        /// </summary>
        /// <param name="download">Download</param>
        public void InsertDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Insert(download);
        }

        /// <summary>
        /// Updates the download
        /// </summary>
        /// <param name="download">Download</param>
        public void UpdateDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            _downloadRepository.Update(download);
        }
        #endregion
    }
}
