using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class DownloadController : BaseNopController
    {
        private IDownloadService _downloadService;

        public DownloadController(IDownloadService downloadService)
        {
            this._downloadService = downloadService;
        }
        
        public ActionResult DownloadFile(int downloadId)
        {
            var download = _downloadService.GetDownloadById(downloadId);
            if (download == null)
                throw new ArgumentException("No download record found with the specified id", "downloadId");

            if (download.UseDownloadUrl)
            {
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                //use stored data
                if (download.DownloadBinary == null)
                    throw new NopException(string.Format("Download data is not available any more. Download ID={0}", downloadId));
                
                string fileName = download.Filename ?? downloadId.ToString();
                return new FileContentResult(download.DownloadBinary, download.ContentType) { FileDownloadName = fileName + download.Extension };
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveDownloadUrl(string downloadUrl)
        {
            //var download = _downloadService.GetDownloadById(downloadId);
            //if (download != null)
            //{
            //    //update
            //    download.UseDownloadUrl = true;
            //    download.DownloadUrl = downloadUrl;
            //    _downloadService.UpdateDownload(download);
            //}
            //else
            //{
            //insert
            var download = new Download()
              {
                  UseDownloadUrl = true,
                  DownloadUrl = downloadUrl,
                  IsNew = true
              };
            _downloadService.InsertDownload(download);
            //}

            return Json(new { downloadId = download.Id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsyncUpload()
        {
            var httpPostedFile = Request.Files[0];
            var download = new Download()
            {
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = httpPostedFile.GetDownloadBits(),
                ContentType = httpPostedFile.ContentType,
                Filename = Path.GetFileNameWithoutExtension(httpPostedFile.FileName),
                Extension = Path.GetExtension(httpPostedFile.FileName),
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            return Json(new { downloadId = download.Id, downloadUrl = Url.Action("DownloadFile", new { downloadId = download.Id }) });
        }
    }
}
