using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller used by jbimages (JustBoil.me) plugin (TimyMCE)
    /// </summary>
    //do not validate request token (XSRF)
    [AdminAntiForgery(true)]
    public partial class JbimagesController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;

        public JbimagesController(IPermissionService permissionService)
        {
            this._permissionService = permissionService;
        }

        protected virtual IList<string> GetAllowedFileTypes()
        {
            return new List<string> {".gif", ".jpg", ".jpeg", ".png", ".bmp"};
        }

        [HttpPost]
        public virtual IActionResult Upload()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
            {
                ViewData["resultCode"] = "failed";
                ViewData["result"] = "No access to this functionality";
                return View();
            }

            if (Request.Form.Files.Count == 0)
                throw new Exception("No file uploaded");

            var uploadFile = Request.Form.Files.FirstOrDefault();
            if (uploadFile == null)
            {
                ViewData["resultCode"] = "failed";
                ViewData["result"] = "No file name provided";
                return View();
            }

            var fileName = Path.GetFileName(uploadFile.FileName);
            if (string.IsNullOrEmpty(fileName))
            {
                ViewData["resultCode"] = "failed";
                ViewData["result"] = "No file name provided";
                return View();
            }

            var directory = "~/wwwroot/images/uploaded/";
            var filePath = Path.Combine(CommonHelper.MapPath(directory), fileName);

            var fileExtension = Path.GetExtension(filePath);
            if (!GetAllowedFileTypes().Contains(fileExtension))
            {
                ViewData["resultCode"] = "failed";
                ViewData["result"] = $"Files with {fileExtension} extension cannot be uploaded";
                return View();
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                uploadFile.CopyTo(fileStream);
            }

            ViewData["resultCode"] = "success";
            ViewData["result"] = "success";
            ViewData["filename"] = this.Url.Content($"{directory}{fileName}");
            return View();
        }
    }
}