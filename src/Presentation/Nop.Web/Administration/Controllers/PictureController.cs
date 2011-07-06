using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    //[AdminAuthorize] Do not use [AdminAuthorzie] attribute because of flash cookie bug used in uploadify
    public class PictureController : BaseNopController
    {
        private readonly IPictureService _pictureService;
        private readonly IAuthenticationService _authenticationService;

        public PictureController(IPictureService pictureService,
            IAuthenticationService authenticationService)
        {
            this._pictureService = pictureService;
            this._authenticationService = authenticationService;
        }

        public ActionResult InsertPicture(HttpPostedFileBase httpPostedFile)
        {
            byte[] pictureBinary = httpPostedFile.GetPictureBits();

            //TODO: find a better solution: little hack here
            //'Uploadify' component uploads all files with "application/octet-stream" mime type
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            string contentType = httpPostedFile.ContentType;
            //string fileExtension = Path.GetExtension(httpPostedFile.FileName);
            //if (!String.IsNullOrEmpty(fileExtension))
            //    fileExtension = fileExtension.ToLowerInvariant();
            //switch (fileExtension)
            //{
            //    case ".bmp":
            //        contentType = "image/bmp";
            //        break;
            //    case ".gif":
            //        contentType = "image/gif";
            //        break;
            //    case ".jpeg":
            //    case ".jpg":
            //    case ".jpe":
            //    case ".jfif":
            //    case ".pjpeg":
            //    case ".pjp":
            //        contentType = "image/jpeg";
            //        break;
            //    case ".png":
            //        contentType = "image/png";
            //        break;
            //    case ".tiff":
            //    case ".tif":
            //        contentType = "image/tiff";
            //        break;
            //    default:
            //        break;
            //}

            var picture = _pictureService.InsertPicture(pictureBinary, contentType, true);
            return Json(new { pictureId = picture.Id, imageUrl = _pictureService.GetPictureUrl(picture, 100) });
        }

        public ActionResult AsyncUpload(IEnumerable<HttpPostedFileBase> attachments)
        {
            var attachment = attachments.ToList().SingleOrDefault();
            if (attachment != null)
            {
                return InsertPicture(attachment);
            }
            return Content("No image provided.");
        }
    }
}
