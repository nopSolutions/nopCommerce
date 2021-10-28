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

        protected IPermissionService PermissionService { get; }
        protected IRoxyFilemanService RoxyFilemanService { get; }

        #endregion

        #region Ctor

        public RoxyFilemanController(
            IPermissionService permissionService,
            IRoxyFilemanService roxyFilemanService)
        {
            PermissionService = permissionService;
            RoxyFilemanService = roxyFilemanService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create configuration file for RoxyFileman
        /// </summary>
        public virtual async Task CreateConfiguration()
        {
            await RoxyFilemanService.CreateConfigurationAsync();
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
                if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.HtmlEditorManagePictures))
                    throw new Exception("You don't have required permission");

                if (!StringValues.IsNullOrEmpty(HttpContext.Request.Query["a"]))
                    action = HttpContext.Request.Query["a"];

                switch (action.ToUpperInvariant())
                {
                    case "DIRLIST":
                        await RoxyFilemanService.GetDirectoriesAsync(HttpContext.Request.Query["type"]);
                        break;
                    case "FILESLIST":
                        await RoxyFilemanService.GetFilesAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["type"]);
                        break;
                    case "COPYDIR":
                        await RoxyFilemanService.CopyDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                        break;
                    case "COPYFILE":
                        await RoxyFilemanService.CopyFileAsync(HttpContext.Request.Query["f"], HttpContext.Request.Query["n"]);
                        break;
                    case "CREATEDIR":
                        await RoxyFilemanService.CreateDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                        break;
                    case "DELETEDIR":
                        await RoxyFilemanService.DeleteDirectoryAsync(HttpContext.Request.Query["d"]);
                        break;
                    case "DELETEFILE":
                        await RoxyFilemanService.DeleteFileAsync(HttpContext.Request.Query["f"]);
                        break;
                    case "DOWNLOAD":
                        await RoxyFilemanService.DownloadFileAsync(HttpContext.Request.Query["f"]);
                        break;
                    case "DOWNLOADDIR":
                        await RoxyFilemanService.DownloadDirectoryAsync(HttpContext.Request.Query["d"]);
                        break;
                    case "MOVEDIR":
                        await RoxyFilemanService.MoveDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                        break;
                    case "MOVEFILE":
                        await RoxyFilemanService.MoveFileAsync(HttpContext.Request.Query["f"], HttpContext.Request.Query["n"]);
                        break;
                    case "RENAMEDIR":
                        await RoxyFilemanService.RenameDirectoryAsync(HttpContext.Request.Query["d"], HttpContext.Request.Query["n"]);
                        break;
                    case "RENAMEFILE":
                        await RoxyFilemanService.RenameFileAsync(HttpContext.Request.Query["f"], HttpContext.Request.Query["n"]);
                        break;
                    case "GENERATETHUMB":
                        await RoxyFilemanService.CreateImageThumbnailAsync(HttpContext.Request.Query["f"]);
                        break;
                    case "UPLOAD":
                        await RoxyFilemanService.UploadFilesAsync(HttpContext.Request.Form["d"]);
                        break;
                    default:
                        await HttpContext.Response.WriteAsync(RoxyFilemanService.GetErrorResponse("This action is not implemented."));
                        break;
                }
            }
            catch (Exception ex)
            {
                if (action == "UPLOAD" && !RoxyFilemanService.IsAjaxRequest())
                    await HttpContext.Response.WriteAsync($"<script>parent.fileUploaded({RoxyFilemanService.GetErrorResponse(await RoxyFilemanService.GetLanguageResourceAsync("E_UploadNoFiles"))});</script>");
                else
                    await HttpContext.Response.WriteAsync(RoxyFilemanService.GetErrorResponse(ex.Message));
            }
        }

        #endregion
    }
}