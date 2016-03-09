using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using Nop.Web.Framework.Security;

namespace Nop.Admin.Controllers
{
    public partial class DownloadController : BaseAdminController
    {
        private readonly IDownloadService _downloadService;

        public DownloadController(IDownloadService downloadService)
        {
            this._downloadService = downloadService;
        }

        public ActionResult DownloadFile(Guid downloadGuid)
        {
            var download = _downloadService.GetDownloadByGuid(downloadGuid);
            if (download == null)
                return Content("No download record found with the specified id");

            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //use stored data
            if (download.DownloadBinary == null)
                return Content(string.Format("Download data is not available any more. Download GD={0}", download.Id));

            string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
            string contentType = !String.IsNullOrWhiteSpace(download.ContentType)
                ? download.ContentType
                : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType)
            {
                FileDownloadName = fileName + download.Extension
            };
        }

        [HttpPost]
        [ValidateInput(false)]
        //do not validate request token (XSRF)
        [AdminAntiForgery(true)] 
        public ActionResult SaveDownloadUrl(string downloadUrl)
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

            return Json(new { downloadId = download.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //do not validate request token (XSRF)
        [AdminAntiForgery(true)]
        public ActionResult AsyncUpload()
        {
            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
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
                downloadUrl = Url.Action("DownloadFile", new { downloadGuid= download.DownloadGuid }) },
                MimeTypes.TextPlain);
        }
    }
}
