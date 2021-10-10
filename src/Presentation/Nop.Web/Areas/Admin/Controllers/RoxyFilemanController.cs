using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Security;

namespace Nop.Web.Areas.Admin.Controllers
{
    //Controller for Roxy fileman (http://www.roxyfileman.com/) for TinyMCE editor
    //the original file was \RoxyFileman-1.4.5-net\fileman\asp_net\main.ashx

    //do not validate request token (XSRF)
    public class RoxyFilemanController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IRoxyFilemanService _roxyFilemanService;

        #endregion

        #region Ctor

        public RoxyFilemanController(
            IPermissionService permissionService,
            IRoxyFilemanService roxyFilemanService)
        {
            _permissionService = permissionService;
            _roxyFilemanService = roxyFilemanService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create configuration file for RoxyFileman
        /// </summary>
        public virtual async Task CreateConfiguration()
        {
            await _roxyFilemanService.CreateConfigurationAsync();
        }

        /// <summary>
        /// Process request
        /// </summary>
        [IgnoreAntiforgeryToken]
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
        [IgnoreAntiforgeryToken]
        protected virtual async Task ProcessRequestAsync()
        {
            var action = "DIRLIST";
            try
            {
                if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.HtmlEditorManagePictures))
                    throw new Exception("You don't have required permission");

                if (!StringValues.IsNullOrEmpty(HttpContext.Request.Query["a"]))
                    action = HttpContext.Request.Query["a"];

                switch (action.ToUpperInvariant())
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
                        await _roxyFilemanService.CreateImageThumbnailAsync(HttpContext.Request.Query["f"]);
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
                    await HttpContext.Response.WriteAsync($"<script>parent.fileUploaded({_roxyFilemanService.GetErrorResponse(await _roxyFilemanService.GetLanguageResourceAsync("E_UploadNoFiles"))});</script>");
                else
                    await HttpContext.Response.WriteAsync(_roxyFilemanService.GetErrorResponse(ex.Message));
            }
        }

        #endregion
    }
}