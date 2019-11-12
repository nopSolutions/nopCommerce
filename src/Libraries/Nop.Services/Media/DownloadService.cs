using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Orders;

namespace Nop.Services.Media
{
    /// <summary>
    /// Download service
    /// </summary>
    public partial class DownloadService : IDownloadService
    {
        #region Fields

        private readonly IEventPublisher _eventPubisher;
        private readonly IRepository<Download> _downloadRepository;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        #endregion

        #region Ctor

        public DownloadService(IEventPublisher eventPubisher,
            IRepository<Download> downloadRepository,
            IOrderService orderService,
            IProductService productService)
        {
            _eventPubisher = eventPubisher;
            _downloadRepository = downloadRepository;
            _orderService = orderService;
            _productService = productService;
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

            return _downloadRepository.GetById(downloadId);
        }

        /// <summary>
        /// Gets a download by GUID
        /// </summary>
        /// <param name="downloadGuid">Download GUID</param>
        /// <returns>Download</returns>
        public virtual Download GetDownloadByGuid(Guid downloadGuid)
        {
            if (downloadGuid == Guid.Empty)
                return null;

            var query = from o in _downloadRepository.Table
                        where o.DownloadGuid == downloadGuid
                        select o;

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Deletes a download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void DeleteDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            _downloadRepository.Delete(download);

            _eventPubisher.EntityDeleted(download);
        }

        /// <summary>
        /// Inserts a download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void InsertDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            _downloadRepository.Insert(download);

            _eventPubisher.EntityInserted(download);
        }

        /// <summary>
        /// Updates the download
        /// </summary>
        /// <param name="download">Download</param>
        public virtual void UpdateDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            _downloadRepository.Update(download);

            _eventPubisher.EntityUpdated(download);
        }

        /// <summary>
        /// Gets a value indicating whether download is allowed
        /// </summary>
        /// <param name="orderItem">Order item to check</param>
        /// <returns>True if download is allowed; otherwise, false.</returns>
        public virtual bool IsDownloadAllowed(OrderItem orderItem)
        {
            if (orderItem is null)
                return false;

            var order = _orderService.GetOrderById(orderItem.OrderId);
            if (order == null || order.Deleted)
                return false;

            //order status
            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            var product = _productService.GetProductById(orderItem.ProductId);

            if (product == null || !product.IsDownload)
                return false;

            //payment status
            switch (product.DownloadActivationType)
            {
                case DownloadActivationType.WhenOrderIsPaid:
                    if (order.PaymentStatus == PaymentStatus.Paid && order.PaidDateUtc.HasValue)
                    {
                        //expiration date
                        if (product.DownloadExpirationDays.HasValue)
                        {
                            if (order.PaidDateUtc.Value.AddDays(product.DownloadExpirationDays.Value) > DateTime.UtcNow)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    
                    break;
                case DownloadActivationType.Manually:
                    if (orderItem.IsDownloadActivated)
                    {
                        //expiration date
                        if (product.DownloadExpirationDays.HasValue)
                        {
                            if (order.CreatedOnUtc.AddDays(product.DownloadExpirationDays.Value) > DateTime.UtcNow)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                   
                    break;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether license download is allowed
        /// </summary>
        /// <param name="orderItem">Order item to check</param>
        /// <returns>True if license download is allowed; otherwise, false.</returns>
        public virtual bool IsLicenseDownloadAllowed(OrderItem orderItem)
        {
            if (orderItem == null)
                return false;

            return IsDownloadAllowed(orderItem) &&
                orderItem.LicenseDownloadId.HasValue &&
                orderItem.LicenseDownloadId > 0;
        }

        /// <summary>
        /// Gets the download binary array
        /// </summary>
        /// <param name="file">File</param>
        /// <returns>Download binary array</returns>
        public virtual byte[] GetDownloadBits(IFormFile file)
        {
            using (var fileStream = file.OpenReadStream())
            {
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    return fileBytes;
                }
            }
        }

        #endregion
    }
}