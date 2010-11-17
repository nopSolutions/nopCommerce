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
using System.IO;
using System.Linq;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Caching;

namespace NopSolutions.NopCommerce.BusinessLogic.Media
{
    /// <summary>
    /// Download service
    /// </summary>
    public partial class DownloadService : IDownloadService
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public DownloadService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets a download url for an admin area
        /// </summary>
        /// <param name="download">Download instance</param>
        /// <returns>Download url</returns>
        public string GetAdminDownloadUrl(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");
            string url = CommonHelper.GetStoreAdminLocation() + "GetDownloadAdmin.ashx?DownloadID=" + download.DownloadId;
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets a download url for a product variant
        /// </summary>
        /// <param name="orderProductVariant">Order product variant instance</param>
        /// <returns>Download url</returns>
        public string GetDownloadUrl(OrderProductVariant orderProductVariant)
        {
            if (orderProductVariant == null)
                throw new ArgumentNullException("orderProductVariant");

            string url = string.Empty;
            var productVariant = orderProductVariant.ProductVariant;
            if (productVariant != null && productVariant.IsDownload)
            {
                url = string.Format("{0}GetDownload.ashx?OrderProductVariantGuid={1}", CommonHelper.GetStoreLocation(), orderProductVariant.OrderProductVariantGuid);
            }
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets a license download url for a product variant
        /// </summary>
        /// <param name="orderProductVariant">Order product variant instance</param>
        /// <returns>Download url</returns>
        public string GetLicenseDownloadUrl(OrderProductVariant orderProductVariant)
        {
            if (orderProductVariant == null)
                throw new ArgumentNullException("orderProductVariant");

            string url = string.Empty;
            var productVariant = orderProductVariant.ProductVariant;
            if (productVariant != null && productVariant.IsDownload && orderProductVariant.LicenseDownloadId > 0)
            {
                url = string.Format("{0}GetLicense.ashx?OrderProductVariantGuid={1}", CommonHelper.GetStoreLocation(), orderProductVariant.OrderProductVariantGuid);
            }
            return url.ToLowerInvariant();
        }
        
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
                url = CommonHelper.GetStoreLocation() + "GetDownload.ashx?SampleDownloadProductVariantID=" + productVariant.ProductVariantId.ToString();
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
            
            
            var query = from d in _context.Downloads
                        where d.DownloadId == downloadId
                        select d;
            var download = query.SingleOrDefault();

            return download;
        }

        /// <summary>
        /// Deletes a download
        /// </summary>
        /// <param name="downloadId">Download identifier</param>
        public void DeleteDownload(int downloadId)
        {
            var download = GetDownloadById(downloadId);
            if (download == null)
                return;

            
            if (!_context.IsAttached(download))
                _context.Downloads.Attach(download);
            _context.DeleteObject(download);
            _context.SaveChanges();
        }

        /// <summary>
        /// Inserts a download
        /// </summary>
        /// <param name="download">Download</param>
        public void InsertDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");
            
            download.DownloadUrl = CommonHelper.EnsureNotNull(download.DownloadUrl);
            download.DownloadUrl = CommonHelper.EnsureMaximumLength(download.DownloadUrl, 400);
            download.ContentType = CommonHelper.EnsureNotNull(download.ContentType);
            download.ContentType = CommonHelper.EnsureMaximumLength(download.ContentType, 20);
            download.Filename = CommonHelper.EnsureNotNull(download.Filename);
            download.Filename = CommonHelper.EnsureMaximumLength(download.Filename, 100);
            download.Extension = CommonHelper.EnsureNotNull(download.Extension);
            download.Extension = CommonHelper.EnsureMaximumLength(download.Extension, 20);

            

            _context.Downloads.AddObject(download);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the download
        /// </summary>
        /// <param name="download">Download</param>
        public void UpdateDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException("download");

            download.DownloadUrl = CommonHelper.EnsureNotNull(download.DownloadUrl);
            download.DownloadUrl = CommonHelper.EnsureMaximumLength(download.DownloadUrl, 400);
            download.ContentType = CommonHelper.EnsureNotNull(download.ContentType);
            download.ContentType = CommonHelper.EnsureMaximumLength(download.ContentType, 20);
            download.Filename = CommonHelper.EnsureNotNull(download.Filename);
            download.Filename = CommonHelper.EnsureMaximumLength(download.Filename, 100);
            download.Extension = CommonHelper.EnsureNotNull(download.Extension);
            download.Extension = CommonHelper.EnsureMaximumLength(download.Extension, 20);

            
            if (!_context.IsAttached(download))
                _context.Downloads.Attach(download);

            _context.SaveChanges();
        }
        #endregion
    }
}
