using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Orders;

namespace Nop.Web.Controllers
{
    public partial class DownloadController : BaseNopController
    {
        private readonly IDownloadService _downloadService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;

        private readonly CustomerSettings _customerSettings;

        public DownloadController(IDownloadService downloadService, IProductService productService,
            IOrderService orderService, IWorkContext workContext, CustomerSettings customerSettings)
        {
            this._downloadService = downloadService;
            this._productService = productService;
            this._orderService = orderService;
            this._workContext = workContext;
            this._customerSettings = customerSettings;
        }
        
        public ActionResult Sample(int productVariantId)
        {
            var productVariant = _productService.GetProductVariantById(productVariantId);
            if (productVariant == null)
                return RedirectToRoute("HomePage");

            if (!productVariant.HasSampleDownload)
                return Content("Product variant doesn't have a sample download.");

            var download = _downloadService.GetDownloadById(productVariant.SampleDownloadId);
            if (download == null)
                return Content("Sample download is not available any more.");

            if (download.UseDownloadUrl)
            {
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");

                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : productVariant.Id.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

        public ActionResult GetDownload(Guid opvId, bool agree = false)
        {
            var orderProductVariant = _orderService.GetOrderProductVariantByGuid(opvId);
            if (orderProductVariant == null)
                return RedirectToRoute("HomePage");

            var order = orderProductVariant.Order;
            var productVariant = orderProductVariant.ProductVariant;
            if (!_downloadService.IsDownloadAllowed(orderProductVariant))
                return Content("Downloads are not allowed");

            if (_customerSettings.DownloadableProductsValidateUser)
            {
                if (_workContext.CurrentCustomer == null)
                    return new HttpUnauthorizedResult();

                if (order.CustomerId != _workContext.CurrentCustomer.Id)
                    return Content("This is not your order");
            }

            var download = _downloadService.GetDownloadById(productVariant.DownloadId);
            if (download == null)
                return Content("Download is not available any more.");

            if (productVariant.HasUserAgreement)
            {
                if (!agree)
                    return RedirectToRoute("DownloadUserAgreement", new { opvid = opvId });
            }


            if (!productVariant.UnlimitedDownloads && orderProductVariant.DownloadCount >= productVariant.MaxNumberOfDownloads)
                return Content(string.Format("You have reached maximum number of downloads {0}", productVariant.MaxNumberOfDownloads));
            

            if (download.UseDownloadUrl)
            {
                //increase download
                orderProductVariant.DownloadCount++;
                _orderService.UpdateOrder(order);

                //return result
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");

                //increase download
                orderProductVariant.DownloadCount++;
                _orderService.UpdateOrder(order);

                //return result
                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : productVariant.Id.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

        public ActionResult GetLicense(Guid opvId)
        {
            var orderProductVariant = _orderService.GetOrderProductVariantByGuid(opvId);
            if (orderProductVariant == null)
                return RedirectToRoute("HomePage");

            var order = orderProductVariant.Order;
            var productVariant = orderProductVariant.ProductVariant;
            if (!_downloadService.IsLicenseDownloadAllowed(orderProductVariant))
                return Content("Downloads are not allowed");

            if (_customerSettings.DownloadableProductsValidateUser)
            {
                if (_workContext.CurrentCustomer == null)
                    return new HttpUnauthorizedResult();

                if (order.CustomerId != _workContext.CurrentCustomer.Id)
                    return Content("This is not your order");
            }

            var download = _downloadService.GetDownloadById(orderProductVariant.LicenseDownloadId.HasValue ? orderProductVariant.LicenseDownloadId.Value : 0);
            if (download == null)
                return Content("Download is not available any more.");
            
            if (download.UseDownloadUrl)
            {
                //return result
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");
                
                //return result
                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : productVariant.Id.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

        public ActionResult GetFileUpload(Guid downloadId)
        {
            var download = _downloadService.GetDownloadByGuid(downloadId);
            if (download == null)
                return Content("Download is not available any more.");

            if (download.UseDownloadUrl)
            {
                //return result
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");

                //return result
                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

    }
}
