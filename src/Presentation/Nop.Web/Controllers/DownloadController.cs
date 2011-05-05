using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Web.Extensions;
using Nop.Web.Models;
using Nop.Web.Models.Home;

namespace Nop.Web.Controllers
{
    public class DownloadController : BaseNopController
    {
        private readonly IDownloadService _downloadService;
        private readonly IProductService _productService;

        public DownloadController(IDownloadService downloadService, IProductService productService)
        {
            this._downloadService = downloadService;
            this._productService = productService;
        }
        
        public ActionResult Sample(int productVariantId)
        {
            var productVariant = _productService.GetProductVariantById(productVariantId);
            if (productVariant == null)
                return RedirectToAction("Index", "Home");

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

                string fileName = download.Filename ?? productVariant.Id.ToString();
                return new FileContentResult(download.DownloadBinary, download.ContentType) { FileDownloadName = fileName + download.Extension };
            }
        }
    }
}
