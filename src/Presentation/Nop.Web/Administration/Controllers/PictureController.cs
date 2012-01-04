using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Nop.Services.Authentication;
using Nop.Services.Media;
using Nop.Services.Security;

namespace Nop.Admin.Controllers
{
    //[AdminAuthorize] Do not use [AdminAuthorzie] attribute because of flash cookie bug used in uploadify
    public class PictureController : BaseNopController
    {
        private readonly IPictureService _pictureService;
        private readonly IPermissionService _permissionService;
        private readonly IAuthenticationService _authenticationService;

        public PictureController(IPictureService pictureService,
             IPermissionService permissionService,
            IAuthenticationService authenticationService)
        {
            this._pictureService = pictureService;
            this._permissionService = permissionService;
            this._authenticationService = authenticationService;
        }

        public ActionResult InsertPicture(string authToken, HttpPostedFileBase httpPostedFile)
        {
            //Workaround for flash cookie bug
            //http://stackoverflow.com/questions/1729179/uploadify-session-and-authentication-with-asp-net-mvc
            //http://geekswithblogs.net/apopovsky/archive/2009/05/06/working-around-flash-cookie-bug-in-asp.net-mvc.aspx

            var ticket = FormsAuthentication.Decrypt(authToken);
            if (ticket == null)
                return Json(new { success = false, error = "No token provided"});

            var identity = new FormsIdentity(ticket);
            if (!identity.IsAuthenticated)
                return Json(new { success = false, error = "User is not authenticated" });
            
            var customer = ((FormsAuthenticationService)_authenticationService).GetAuthenticatedCustomerFromTicket(ticket);
            if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures, customer))
                return Json(new { success = false, error = "User doesn't have required permissions" });

            byte[] pictureBinary = httpPostedFile.GetPictureBits();



            //TODO: find a better solution: little hack here
            //'Uploadify' component uploads all files with "application/octet-stream" mime type
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            string contentType = httpPostedFile.ContentType;
            string fileExtension = Path.GetExtension(httpPostedFile.FileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            switch (fileExtension)
            {
                case ".bmp":
                    contentType = "image/bmp";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".jpeg":
                case ".jpg":
                case ".jpe":
                case ".jfif":
                case ".pjpeg":
                case ".pjp":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".tiff":
                case ".tif":
                    contentType = "image/tiff";
                    break;
                default:
                    break;
            }

            var picture = _pictureService.InsertPicture(pictureBinary, contentType, null, true);
            return Json(new { success = true, pictureId = picture.Id, imageUrl = _pictureService.GetPictureUrl(picture, 100) });
        }

        public ActionResult AsyncUpload(string authToken)
        {
            return InsertPicture(authToken, Request.Files[0]);
        }
    }
}
