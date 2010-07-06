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

namespace NopSolutions.NopCommerce.BusinessLogic.Media
{
    /// <summary>
    /// Download manager
    /// </summary>
    public partial class DownloadManager
    {
        #region Methods
        /// <summary>
        /// Gets a download url for an admin area
        /// </summary>
        /// <param name="download">Download instance</param>
        /// <returns>Download url</returns>
        public static string GetAdminDownloadUrl(Download download)
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
        public static string GetDownloadUrl(OrderProductVariant orderProductVariant)
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
        public static string GetLicenseDownloadUrl(OrderProductVariant orderProductVariant)
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
        public static string GetSampleDownloadUrl(ProductVariant productVariant)
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
        public static Download GetDownloadById(int downloadId)
        {
            if (downloadId == 0)
                return null;
            
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from d in context.Downloads
                        where d.DownloadId == downloadId
                        select d;
            var download = query.SingleOrDefault();

            return download;
        }

        /// <summary>
        /// Deletes a download
        /// </summary>
        /// <param name="downloadId">Download identifier</param>
        public static void DeleteDownload(int downloadId)
        {
            var download = GetDownloadById(downloadId);
            if (download == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(download))
                context.Downloads.Attach(download);
            context.DeleteObject(download);
            context.SaveChanges();
        }

        /// <summary>
        /// Inserts a download
        /// </summary>
        /// <param name="useDownloadUrl">The value indicating whether DownloadURL property should be used</param>
        /// <param name="downloadUrl">The download URL</param>
        /// <param name="downloadBinary">The download binary</param>
        /// <param name="contentType">The content type</param>
        /// <param name="filename">The filename of the download</param>
        /// <param name="extension">The extension</param>
        /// <param name="isNew">A value indicating whether the download is new</param>
        /// <returns>Download</returns>
        public static Download InsertDownload(bool useDownloadUrl, string downloadUrl,
            byte[] downloadBinary, string contentType, string filename,
            string extension, bool isNew)
        {
            if (downloadUrl == null)
                downloadUrl = string.Empty;
            if (filename == null)
                filename = string.Empty;
            if (contentType == null)
                contentType = string.Empty;
            if (extension == null)
                extension = string.Empty;

            downloadUrl = CommonHelper.EnsureMaximumLength(downloadUrl, 400);
            contentType = CommonHelper.EnsureMaximumLength(contentType, 20);
            filename = CommonHelper.EnsureMaximumLength(filename, 100);
            extension = CommonHelper.EnsureMaximumLength(extension, 20);

            var context = ObjectContextHelper.CurrentObjectContext;
            var download = context.Downloads.CreateObject();
            download.UseDownloadUrl = useDownloadUrl;
            download.DownloadUrl = downloadUrl;
            download.DownloadBinary = downloadBinary;
            download.ContentType = contentType;
            download.Filename = filename;
            download.Extension = extension;
            download.IsNew = isNew;

            context.Downloads.AddObject(download);
            context.SaveChanges();

            return download;
        }

        /// <summary>
        /// Updates the download
        /// </summary>
        /// <param name="downloadId">The download identifier</param>
        /// <param name="useDownloadUrl">The value indicating whether DownloadURL property should be used</param>
        /// <param name="downloadUrl">The download URL</param>
        /// <param name="downloadBinary">The download binary</param>
        /// <param name="contentType">The content type</param>
        /// <param name="filename">The filename of the download</param>
        /// <param name="extension">The extension</param>
        /// <param name="isNew">A value indicating whether the download is new</param>
        /// <returns>Download</returns>
        public static Download UpdateDownload(int downloadId,
            bool useDownloadUrl, string downloadUrl,
            byte[] downloadBinary, string contentType, string filename,
            string extension, bool isNew)
        {
            if (downloadUrl == null)
                downloadUrl = string.Empty;
            if (filename == null)
                filename = string.Empty;
            if (contentType == null)
                contentType = string.Empty;
            if (extension == null)
                extension = string.Empty;

            downloadUrl = CommonHelper.EnsureMaximumLength(downloadUrl, 400);
            contentType = CommonHelper.EnsureMaximumLength(contentType, 20);
            filename = CommonHelper.EnsureMaximumLength(filename, 100);
            extension = CommonHelper.EnsureMaximumLength(extension, 20);

            var download = GetDownloadById(downloadId);
            if (download == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(download))
                context.Downloads.Attach(download);

            download.UseDownloadUrl = useDownloadUrl;
            download.DownloadUrl = downloadUrl;
            download.DownloadBinary = downloadBinary;
            download.ContentType = contentType;
            download.Filename = filename;
            download.Extension = extension;
            download.IsNew = isNew;
            context.SaveChanges();

            return download;
        }

        /// <summary>
        /// Gets the download binary array
        /// </summary>
        /// <param name="fs">File stream</param>
        /// <param name="size">Download size</param>
        /// <returns>Download binary array</returns>
        public static byte[] GetDownloadBits(Stream fs, int size)
        {
            byte[] binary = new byte[size];
            fs.Read(binary, 0, size);
            return binary;
        }
        #endregion
    }
}
