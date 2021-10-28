using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Services.Media;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class DownloadController : BaseAdminController
    {
        #region Fields

        protected IDownloadService DownloadService { get; }
        protected ILogger Logger { get; }
        protected INopFileProvider FileProvider { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public DownloadController(IDownloadService downloadService,
            ILogger logger,
            INopFileProvider fileProvider,
            IWorkContext workContext)
        {
            DownloadService = downloadService;
            Logger = logger;
            FileProvider = fileProvider;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> DownloadFile(Guid downloadGuid)
        {
            var download = await DownloadService.GetDownloadByGuidAsync(downloadGuid);
            if (download == null)
                return Content("No download record found with the specified id");

            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
            //In this case, it is not relevant. Url may not be local.
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
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> SaveDownloadUrl(string downloadUrl)
        {
            //don't allow to save empty download object
            if (string.IsNullOrEmpty(downloadUrl))
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter URL"
                });
            }

            //insert
            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = true,
                DownloadUrl = downloadUrl,
                IsNew = true
            };
            await DownloadService.InsertDownloadAsync(download);

            return Json(new { success = true, downloadId = download.Id });
        }

        [HttpPost]
        //do not validate request token (XSRF)
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> AsyncUpload()
        {
            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded"
                });
            }

            var fileBinary = await DownloadService.GetDownloadBitsAsync(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = FileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = FileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = FileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };

            try
            {
                await DownloadService.InsertDownloadAsync(download);

                //when returning JSON the mime-type must be set to text/plain
                //otherwise some browsers will pop-up a "Save As" dialog.
                return Json(new
                {
                    success = true,
                    downloadId = download.Id,
                    downloadUrl = Url.Action("DownloadFile", new { downloadGuid = download.DownloadGuid })
                });
            }
            catch (Exception exc)
            {
                await Logger.ErrorAsync(exc.Message, exc, await WorkContext.GetCurrentCustomerAsync());

                return Json(new
                {
                    success = false,
                    message = "File cannot be saved"
                });
            }
        }

        #endregion
    }
}