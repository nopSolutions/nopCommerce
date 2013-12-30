using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    /// <summary>
    /// Controller used by jbimages (JustBoil.me) plugin (TimyMVC)
    /// </summary>
    [AdminAuthorize]
    public partial class JbimagesController : BaseNopController
    {
        private readonly IPermissionService _permissionService;

        public JbimagesController(IPermissionService permissionService)
        {
            this._permissionService = permissionService;
        }

        [HttpPost]
        public ActionResult Upload()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Content("No access to this functionality");
            //if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
            //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            //Stream stream = null;
            //var fileName = "";
            //var contentType = "";
            //if (String.IsNullOrEmpty(Request["qqfile"]))
            //{
            //    // IE
            //    HttpPostedFileBase httpPostedFile = Request.Files[0];
            //    if (httpPostedFile == null)
            //        throw new ArgumentException("No file uploaded");
            //    stream = httpPostedFile.InputStream;
            //    fileName = Path.GetFileName(httpPostedFile.FileName);
            //    contentType = httpPostedFile.ContentType;
            //}
            //else
            //{
            //    //Webkit, Mozilla
            //    stream = Request.InputStream;
            //    fileName = Request["qqfile"];
            //}

            //var fileBinary = new byte[stream.Length];
            //stream.Read(fileBinary, 0, fileBinary.Length);

            //var fileExtension = Path.GetExtension(fileName);
            //if (!String.IsNullOrEmpty(fileExtension))
            //    fileExtension = fileExtension.ToLowerInvariant();

            var absoluteUrl = "http://www.nopcommerce.com/App_Themes/darkOrange/images/logo.png";
            return Content(string.Format("<script>top.$('.mce-btn.mce-open').parent().find('.mce-textbox').val('{0}');</script>", absoluteUrl));

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            //return Json(new { success = true, pictureId = picture.Id,
            //    imageUrl = _pictureService.GetPictureUrl(picture, 100) },
            //    "text/plain");
        }
    }
}
