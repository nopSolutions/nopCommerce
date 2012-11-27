//Contributor: http://aspnetadvimage.codeplex.com/
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.UI.Editor;

namespace Nop.Admin.Controllers
{
    /// <summary>
    /// Controller used by netadvimage plugin (TimyMVC)
    /// </summary>
    [AdminAuthorize]
    public partial class NetAdvImageController : BaseNopController
    {
        private readonly IPermissionService _permissionService;
        private readonly INetAdvImageService _imageService;
        private readonly INetAdvDirectoryService _directoryService;
        private readonly HttpContextBase _httpContext;
        
        public NetAdvImageController(IPermissionService permissionService,
            INetAdvImageService imageService, 
            INetAdvDirectoryService directoryService,
            HttpContextBase httpContext)
        {
            this._permissionService = permissionService;
            this._imageService = imageService;
            this._directoryService = directoryService;
            this._httpContext = httpContext;
        }

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Index(string path)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Json(new { success = false, message = "No access to this functionality" }, "application/json");

            string filePath = "";

            try
            {
                // get the name from qqfile url parameter here
                var stream = Request.InputStream;
                if (String.IsNullOrEmpty(Request["qqfile"]))
                {
                    //IE hack here - http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
                    HttpPostedFileBase postedFile = Request.Files[0];
                    stream = postedFile.InputStream;

                    // IE
                    filePath = Path.Combine(path, Path.GetFileName(postedFile.FileName));
                }
                else
                {
                    // Webkit, Mozilla
                    filePath = Path.Combine(path, Request["qqfile"]);
                }

                try
                {
                    int length = 4096;
                    int bytesRead = 0;
                    var buffer = new Byte[length];
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        do
                        {
                            bytesRead = stream.Read(buffer, 0, length);
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                        while (bytesRead > 0);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    // log error hinting to set the write permission of ASPNET or the identity accessing the code
                    return Json(new { success = false, message = ex.Message }, "application/json");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, "application/json");
            }

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new { success = true }, "text/html");
        }

        [HttpPost]
        public JsonResult _GetImages(string path)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Json(new { success = false, message = "No access to this functionality" }, "application/json");

            return new JsonResult { Data = _imageService.GetImages(path, HttpContext) };
        }

        [HttpPost]
        public JsonResult _DeleteImage(string path, string name)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Json(new { success = false, message = "No access to this functionality" }, "application/json");

            return new JsonResult { Data = _imageService.DeleteImage(path, name) };
        }

        [HttpPost]
        public JsonResult _MoveDirectory(string path, string destinationPath)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Json(new { success = false, message = "No access to this functionality" }, "application/json");

            return new JsonResult { Data = _directoryService.MoveDirectory(path, destinationPath) };
        }

        [HttpPost]
        public JsonResult _DeleteDirectory(string path)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Json(new { success = false, message = "No access to this functionality" }, "application/json");

            return new JsonResult { Data = _directoryService.DeleteDirectory(path, HttpContext) };
        }

        [HttpPost]
        public JsonResult _AddDirectory(string path)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Json(new { success = false, message = "No access to this functionality" }, "application/json");

            // If no destination folder was selected, add new folder to root upload path
            if (String.IsNullOrEmpty(path))
                path = Server.MapPath(NetAdvImageSettings.UploadPath);

            _ExpandDirectoryState(path);
            return new JsonResult { Data = _directoryService.AddDirectory(path) };
        }

        [HttpPost]
        public JsonResult _GetDirectories()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Json(new { success = false, message = "No access to this functionality" }, "application/json");

            // Ensure the upload directory exists
            _directoryService.CreateUploadDirectory(HttpContext);

            return new JsonResult { Data = _directoryService.GetDirectoryTree(HttpContext) };
        }

        [HttpPost]
        public JsonResult _RenameDirectory(string path, string name)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Json(new { success = false, message = "No access to this functionality" }, "application/json");

            return new JsonResult { Data = _directoryService.RenameDirectory(path, name, HttpContext) };
        }

        [HttpPost]
        public ActionResult _ExpandDirectoryState(string value)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Json(new { success = false, message = "No access to this functionality" }, "application/json");

            // Get or initalize list in session
            if (_httpContext.Session[NetAdvImageSettings.TreeStateSessionKey] == null)
                _httpContext.Session[NetAdvImageSettings.TreeStateSessionKey] = new List<string>();
            var expandedNodes = _httpContext.Session[NetAdvImageSettings.TreeStateSessionKey] as List<string>;

            // Persist the expanded state of the directory in session
            if (!expandedNodes.Contains(value))
            {
                expandedNodes.Add(value);
                _httpContext.Session[NetAdvImageSettings.TreeStateSessionKey] = expandedNodes;
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult _CollapseDirectoryState(string value)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                return Json(new { success = false, message = "No access to this functionality" }, "application/json");

            // Get or initalize list in session
            if (_httpContext.Session[NetAdvImageSettings.TreeStateSessionKey] == null)
                _httpContext.Session[NetAdvImageSettings.TreeStateSessionKey] = new List<string>();
            var expandedNodes = _httpContext.Session[NetAdvImageSettings.TreeStateSessionKey] as List<string>;

            // Persist the collapsed state of the directory in session
            if (expandedNodes.Contains(value))
            {
                expandedNodes.Remove(value);
                _httpContext.Session[NetAdvImageSettings.TreeStateSessionKey] = expandedNodes;
            }

            return new EmptyResult();
        }
    }
}
