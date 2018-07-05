using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Infrastructure;
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
        #region Fields

        private readonly INopFileProvider _fileProvider;
        private readonly IPermissionService _permissionService;
       
        #endregion
        
        public JbimagesController(INopFileProvider fileProvider,
            IPermissionService permissionService)
        {
            this._fileProvider = fileProvider;
            this._permissionService = permissionService;
        }

        protected virtual IList<string> GetAllowedFileTypes()
        {
            return new List<string> { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
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

            var fileName = _fileProvider.GetFileName(uploadFile.FileName);
            if (string.IsNullOrEmpty(fileName))
            {
                ViewData["resultCode"] = "failed";
                ViewData["result"] = "No file name provided";
                return View();
            }

            var directory = "~/wwwroot/images/uploaded/";
            var filePath = _fileProvider.Combine(_fileProvider.MapPath(directory), fileName);

            var fileExtension = _fileProvider.GetFileExtension(filePath);
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
            ViewData["filename"] = Url.Content($"{directory}{fileName}");
            return View();
        }
    }
}