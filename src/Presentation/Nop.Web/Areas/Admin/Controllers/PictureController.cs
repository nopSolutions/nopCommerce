using Microsoft.AspNetCore.Mvc;
using Nop.Core.Http.Extensions;
using Nop.Services.Media;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class PictureController : BaseAdminController
{
    #region Fields

    protected readonly IPictureService _pictureService;

    #endregion

    #region Ctor

    public PictureController(IPictureService pictureService)
    {
        _pictureService = pictureService;
    }

    #endregion

    #region Methods

    [HttpPost]
    //do not validate request token (XSRF)
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> AsyncUpload()
    {
        //if (!await _permissionService.Authorize(StandardPermission.UploadPictures))
        //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

        var httpPostedFile = await Request.GetFirstOrDefaultFileAsync();
        if (httpPostedFile == null)
            return Json(new { success = false, message = "No file uploaded" });

        var picture = await _pictureService.InsertPictureAsync(httpPostedFile);

        //when returning JSON the mime-type must be set to text/plain
        //otherwise some browsers will pop-up a "Save As" dialog.

        if (picture == null)
            return Json(new { success = false, message = "Wrong file format" });

        return Json(new
        {
            success = true,
            pictureId = picture.Id,
            imageUrl = (await _pictureService.GetPictureUrlAsync(picture, 100)).Url
        });
    }

    #endregion
}