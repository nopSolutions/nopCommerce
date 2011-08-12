using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Security;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Services;
using Nop.Services.Authentication;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    //[AdminAuthorize] Do not use [AdminAuthorzie] attribute because of flash cookie bug used in uploadify
    //we apply it only to requried methods (e.g. DownloadFile and SaveDownloadUrl)
    public class DownloadController : BaseNopController
    {
        private readonly IDownloadService _downloadService;
        private readonly IAuthenticationService _authenticationService;

        public DownloadController(IDownloadService downloadService, IAuthenticationService authenticationService)
        {
            this._downloadService = downloadService;
            this._authenticationService = authenticationService;
        }

        [AdminAuthorize]
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
        [AdminAuthorize]
        public ActionResult SaveDownloadUrl(string downloadUrl)
        {
            //insert
            var download = new Download()
              {
                  UseDownloadUrl = true,
                  DownloadUrl = downloadUrl,
                  IsNew = true
              };
            _downloadService.InsertDownload(download);

            return Json(new { downloadId = download.Id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsyncUpload(string authToken)
        {
            var httpPostedFile = Request.Files[0];
            if (httpPostedFile == null)
                throw new ArgumentException("No file uploaded");

            //Workaround for flash cookie bug
            //http://stackoverflow.com/questions/1729179/uploadify-session-and-authentication-with-asp-net-mvc
            //http://geekswithblogs.net/apopovsky/archive/2009/05/06/working-around-flash-cookie-bug-in-asp.net-mvc.aspx

            var ticket = FormsAuthentication.Decrypt(authToken);
            if (ticket == null)
                throw new Exception("No token provided");

            var identity = new FormsIdentity(ticket);
            if (!identity.IsAuthenticated)
                throw new Exception("User is not authenticated");

            var customer = ((FormsAuthenticationService)_authenticationService).GetAuthenticatedCustomerFromTicket(ticket);
            if (!customer.IsAdmin())
                throw new Exception("User is not admin");


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
