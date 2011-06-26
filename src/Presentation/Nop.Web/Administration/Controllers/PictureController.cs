using System.Web;
using System.Web.Mvc;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class PictureController : BaseNopController
    {
        private readonly IPictureService _pictureService;

        public PictureController(IPictureService pictureService)
        {
            _pictureService = pictureService;
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
