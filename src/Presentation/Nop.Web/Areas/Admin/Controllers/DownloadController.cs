using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class DownloadController : BaseAdminController
    {
        #region Fields

        private readonly IDownloadService _downloadService;

        #endregion

        #region Ctor

        public DownloadController(IDownloadService downloadService)
        {
            this._downloadService = downloadService;
        }

        #endregion

        #region Methods

        public virtual IActionResult DownloadFile(Guid downloadGuid)
        {
            var download = _downloadService.GetDownloadByGuid(downloadGuid);
            if (download == null)
                return Content("No download record found with the specified id");

            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //use stored data
            if (download.DownloadBinary == null)
                return Content($"Download data is not available any more. Download GD={download.Id}");

            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType)
                ? download.ContentType
                : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType)
            {
                FileDownloadName = fileName + download.Extension
            };
        }

        [HttpPost]
        //do not validate request token (XSRF)
        [AdminAntiForgery(true)] 
        public virtual IActionResult SaveDownloadUrl(string downloadUrl)
        {
            //insert
            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = true,
                DownloadUrl = downloadUrl,
                IsNew = true
              };
            _downloadService.InsertDownload(download);

            return Json(new { downloadId = download.Id });
        }

        [HttpPost]
        //do not validate request token (XSRF)
        [AdminAntiForgery(true)]
        public virtual IActionResult AsyncUpload()
        {
            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty,
                });
            }

            var fileBinary = httpPostedFile.GetDownloadBits();

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = Path.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new { success = true, 
                downloadId = download.Id, 
                downloadUrl = Url.Action("DownloadFile", new { downloadGuid= download.DownloadGuid }) });
        }

        #endregion
    }
}