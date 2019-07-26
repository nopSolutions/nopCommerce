using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Security;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    //Controller for Roxy fileman (http://www.roxyfileman.com/) for TinyMCE editor
    //the original file was \RoxyFileman-1.4.5-net\fileman\asp_net\main.ashx

    //do not validate request token (XSRF)
    [AdminAntiForgery(true)]
    public class RoxyFilemanController : BaseAdminController
    {
        #region Fields

        private readonly INopFileProvider _fileProvider;
        private readonly IPermissionService _permissionService;
        private readonly IRoxyFilemanService _roxyFilemanService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public RoxyFilemanController(
            INopFileProvider fileProvider,
            IPermissionService permissionService,
            IRoxyFilemanService roxyFilemanService,
            IWorkContext workContext)
        {
            _fileProvider = fileProvider;
            _permissionService = permissionService;
            _roxyFilemanService = roxyFilemanService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create configuration file for RoxyFileman
        /// </summary>
        public virtual void CreateConfiguration()
        {
            var filePath = _roxyFilemanService.GetConfigurationFilePath();

            //create file if not exists
            _fileProvider.CreateFile(filePath);

            //try to read existing configuration
            var existingText = _fileProvider.ReadAllText(filePath, Encoding.UTF8);
            var existingConfiguration = JsonConvert.DeserializeAnonymousType(existingText, new
            {
                FILES_ROOT = string.Empty,
                SESSION_PATH_KEY = string.Empty,
                THUMBS_VIEW_WIDTH = string.Empty,
                THUMBS_VIEW_HEIGHT = string.Empty,
                PREVIEW_THUMB_WIDTH = string.Empty,
                PREVIEW_THUMB_HEIGHT = string.Empty,
                MAX_IMAGE_WIDTH = string.Empty,
                MAX_IMAGE_HEIGHT = string.Empty,
                DEFAULTVIEW = string.Empty,
                FORBIDDEN_UPLOADS = string.Empty,
                ALLOWED_UPLOADS = string.Empty,
                FILEPERMISSIONS = string.Empty,
                DIRPERMISSIONS = string.Empty,
                LANG = string.Empty,
                DATEFORMAT = string.Empty,
                OPEN_LAST_DIR = string.Empty,
                INTEGRATION = string.Empty,
                RETURN_URL_PREFIX = string.Empty,
                DIRLIST = string.Empty,
                CREATEDIR = string.Empty,
                DELETEDIR = string.Empty,
                MOVEDIR = string.Empty,
                COPYDIR = string.Empty,
                RENAMEDIR = string.Empty,
                FILESLIST = string.Empty,
                UPLOAD = string.Empty,
                DOWNLOAD = string.Empty,
                DOWNLOADDIR = string.Empty,
                DELETEFILE = string.Empty,
                MOVEFILE = string.Empty,
                COPYFILE = string.Empty,
                RENAMEFILE = string.Empty,
                GENERATETHUMB = string.Empty
            });

            //check whether the path base has changed, otherwise there is no need to overwrite the configuration file
            var currentPathBase = HttpContext.Request.PathBase.ToString();
            if (existingConfiguration?.RETURN_URL_PREFIX?.Equals(currentPathBase) ?? false)
                return;

            //create configuration
            var configuration = new
            {
                FILES_ROOT = existingConfiguration?.FILES_ROOT ?? NopRoxyFilemanDefaults.DefaultRootDirectory,
                SESSION_PATH_KEY = existingConfiguration?.SESSION_PATH_KEY ?? string.Empty,
                THUMBS_VIEW_WIDTH = existingConfiguration?.THUMBS_VIEW_WIDTH ?? "140",
                THUMBS_VIEW_HEIGHT = existingConfiguration?.THUMBS_VIEW_HEIGHT ?? "120",
                PREVIEW_THUMB_WIDTH = existingConfiguration?.PREVIEW_THUMB_WIDTH ?? "300",
                PREVIEW_THUMB_HEIGHT = existingConfiguration?.PREVIEW_THUMB_HEIGHT ?? "200",
                MAX_IMAGE_WIDTH = existingConfiguration?.MAX_IMAGE_WIDTH ?? "1000",
                MAX_IMAGE_HEIGHT = existingConfiguration?.MAX_IMAGE_HEIGHT ?? "1000",
                DEFAULTVIEW = existingConfiguration?.DEFAULTVIEW ?? "list",
                FORBIDDEN_UPLOADS = existingConfiguration?.FORBIDDEN_UPLOADS ?? "zip js jsp jsb mhtml mht xhtml xht php phtml " +
                    "php3 php4 php5 phps shtml jhtml pl sh py cgi exe application gadget hta cpl msc jar vb jse ws wsf wsc wsh " +
                    "ps1 ps2 psc1 psc2 msh msh1 msh2 inf reg scf msp scr dll msi vbs bat com pif cmd vxd cpl htpasswd htaccess",
                ALLOWED_UPLOADS = existingConfiguration?.ALLOWED_UPLOADS ?? string.Empty,
                FILEPERMISSIONS = existingConfiguration?.FILEPERMISSIONS ?? "0644",
                DIRPERMISSIONS = existingConfiguration?.DIRPERMISSIONS ?? "0755",
                LANG = existingConfiguration?.LANG ?? _workContext.WorkingLanguage.UniqueSeoCode,
                DATEFORMAT = existingConfiguration?.DATEFORMAT ?? "dd/MM/yyyy HH:mm",
                OPEN_LAST_DIR = existingConfiguration?.OPEN_LAST_DIR ?? "yes",

                //no need user to configure
                INTEGRATION = "tinymce4",
                RETURN_URL_PREFIX = currentPathBase,
                DIRLIST = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=DIRLIST",
                CREATEDIR = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=CREATEDIR",
                DELETEDIR = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=DELETEDIR",
                MOVEDIR = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=MOVEDIR",
                COPYDIR = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=COPYDIR",
                RENAMEDIR = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=RENAMEDIR",
                FILESLIST = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=FILESLIST",
                UPLOAD = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=UPLOAD",
                DOWNLOAD = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=DOWNLOAD",
                DOWNLOADDIR = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=DOWNLOADDIR",
                DELETEFILE = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=DELETEFILE",
                MOVEFILE = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=MOVEFILE",
                COPYFILE = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=COPYFILE",
                RENAMEFILE = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=RENAMEFILE",
                GENERATETHUMB = $"{HttpContext.Request.PathBase}/Admin/RoxyFileman/ProcessRequest?a=GENERATETHUMB"
            };

            //save the file
            var text = JsonConvert.SerializeObject(configuration, Formatting.Indented);
            _fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Process request
        /// </summary>
        public virtual void ProcessRequest()
        {
            //async requests are disabled in the js code, so use .Wait() method here
            ProcessRequestAsync().Wait();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Process the incoming request
        /// </summary>
        /// <returns>A task that represents the completion of the operation</returns>
        protected virtual async Task ProcessRequestAsync()
        {
            var action = "DIRLIST";
            try
            {
                if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                    throw new Exception("You don't have required permission");

                if (!StringValues.IsNullOrEmpty(HttpContext.Request.Query["a"]))
                    action = HttpContext.Request.Query["a"];

                switch (action.ToUpper())
                {
                    case "DIRLIST":
                        await _roxyFilemanService.GetDirectoriesAsync(HttpContext.Request.Query["type"]);
                        break;
                    case "FILESLIST":
                        await _roxyFilemanService.GetFilesAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["type"]);
                        break;
                    case "COPYDIR":
                        await _roxyFilemanService.CopyDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                        break;
                    case "COPYFILE":
                        await _roxyFilemanService.CopyFileAsync(HttpContext.Request.Query["f"], HttpContext.Request.Query["n"]);
                        break;
                    case "CREATEDIR":
                        await _roxyFilemanService.CreateDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                        break;
                    case "DELETEDIR":
                        await _roxyFilemanService.DeleteDirectoryAsync(HttpContext.Request.Query["d"]);
                        break;
                    case "DELETEFILE":
                        await _roxyFilemanService.DeleteFileAsync(HttpContext.Request.Query["f"]);
                        break;
                    case "DOWNLOAD":
                        await _roxyFilemanService.DownloadFileAsync(HttpContext.Request.Query["f"]);
                        break;
                    case "DOWNLOADDIR":
                        await _roxyFilemanService.DownloadDirectoryAsync(HttpContext.Request.Query["d"]);
                        break;
                    case "MOVEDIR":
                        await _roxyFilemanService.MoveDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                        break;
                    case "MOVEFILE":
                        await _roxyFilemanService.MoveFileAsync(HttpContext.Request.Query["f"], HttpContext.Request.Query["n"]);
                        break;
                    case "RENAMEDIR":
                        await _roxyFilemanService.RenameDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                        break;
                    case "RENAMEFILE":
                        await _roxyFilemanService.RenameFileAsync(HttpContext.Request.Query["f"], HttpContext.Request.Query["n"]);
                        break;
                    case "GENERATETHUMB":
                        _roxyFilemanService.CreateImageThumbnail(HttpContext.Request.Query["f"]);
                        break;
                    case "UPLOAD":
                        await _roxyFilemanService.UploadFilesAsync(HttpContext.Request.Form["d"]);
                        break;
                    default:
                        await HttpContext.Response.WriteAsync(_roxyFilemanService.GetErrorResponse("This action is not implemented."));
                        break;
                }
            }
            catch (Exception ex)
            {
                if (action == "UPLOAD" && !_roxyFilemanService.IsAjaxRequest())
                    await HttpContext.Response.WriteAsync($"<script>parent.fileUploaded({_roxyFilemanService.GetErrorResponse(_roxyFilemanService.GetLanguageResource("E_UploadNoFiles"))});</script>");
                else
                    await HttpContext.Response.WriteAsync(_roxyFilemanService.GetErrorResponse(ex.Message));
            }
        }

        #endregion
    }
}