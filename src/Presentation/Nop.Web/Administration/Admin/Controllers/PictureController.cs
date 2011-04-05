using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Services.Media;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class PictureController : BaseNopController
    {
        private IPictureService _pictureService;

        public PictureController(IPictureService pictureService)
        {
            _pictureService = pictureService;
        }

        public ActionResult Render(int pictureId, int targetSize = 100)
        {
            var picture = _pictureService.GetPictureById(pictureId);

            if (picture == null) { 
                //TODO:Return default image 
            }

            return new PictureResult(picture, targetSize);
        }

        public ActionResult InsertPicture(HttpPostedFileBase httpPostedFile)
        {
             byte[] pictureBinary = httpPostedFile.GetPictureBits();
             var picture = _pictureService.InsertPicture(pictureBinary, httpPostedFile.ContentType, true);
             return Json(new { pictureId = picture.Id, imageUrl = _pictureService.GetPictureUrl(picture, 100)});
        }

        public ActionResult AsyncUpload()
        {
            return InsertPicture(Request.Files[0]);
        }
    }
}
