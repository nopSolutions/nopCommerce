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
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor

        public RoxyFilemanController(
            IPermissionService permissionService,
            IRoxyFilemanService roxyFilemanService,
            IHttpContextAccessor httpContextAccessor)
        {
            _permissionService = permissionService;
            _roxyFilemanService = roxyFilemanService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create configuration file for RoxyFileman
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task CreateConfiguration()
        {
            await _roxyFilemanService.CreateConfigurationAsync();
        }

        /// <summary>
        /// Process the incoming request
        /// </summary>

        /// <returns>A task that represents the asynchronous operation</returns>
        [IgnoreAntiforgeryToken]
        public virtual async Task ProcessRequest()
        {
            var action = "DIRLIST";
            var httpContext = _httpContextAccessor.HttpContext;
            try
            {
                if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.HtmlEditorManagePictures))
                    throw new Exception("You don't have required permission");

                if (!StringValues.IsNullOrEmpty(httpContext.Request.Query["a"]))
                    action = httpContext.Request.Query["a"];

                switch (action.ToUpper())
                {
                    case "DIRLIST":
                        await _roxyFilemanService.GetDirectoriesAsync(httpContext.Request.Query["type"]);
                        break;
                    case "FILESLIST":
                        await _roxyFilemanService.GetFilesAsync(httpContext.Request.Query["d"], httpContext.Request.Query["type"]);
                        break;
                    case "COPYDIR":
                        await _roxyFilemanService.CopyDirectoryAsync(httpContext.Request.Query["d"], httpContext.Request.Query["n"]);
                        break;
                    case "COPYFILE":
                        await _roxyFilemanService.CopyFileAsync(httpContext.Request.Query["f"], httpContext.Request.Query["n"]);
                        break;
                    case "CREATEDIR":
                        await _roxyFilemanService.CreateDirectoryAsync(httpContext.Request.Query["d"], httpContext.Request.Query["n"]);
                        break;
                    case "DELETEDIR":
                        await _roxyFilemanService.DeleteDirectoryAsync(httpContext.Request.Query["d"]);
                        break;
                    case "DELETEFILE":
                        await _roxyFilemanService.DeleteFileAsync(httpContext.Request.Query["f"]);
                        break;
                    case "DOWNLOAD":
                        await _roxyFilemanService.DownloadFileAsync(httpContext.Request.Query["f"]);
                        break;
                    case "DOWNLOADDIR":
                        await _roxyFilemanService.DownloadDirectoryAsync(httpContext.Request.Query["d"]);
                        break;
                    case "MOVEDIR":
                        await _roxyFilemanService.MoveDirectoryAsync(httpContext.Request.Query["d"], httpContext.Request.Query["n"]);
                        break;
                    case "MOVEFILE":
                        await _roxyFilemanService.MoveFileAsync(httpContext.Request.Query["f"], httpContext.Request.Query["n"]);
                        break;
                    case "RENAMEDIR":
                        await _roxyFilemanService.RenameDirectoryAsync(httpContext.Request.Query["d"], httpContext.Request.Query["n"]);
                        break;
                    case "RENAMEFILE":
                        await _roxyFilemanService.RenameFileAsync(httpContext.Request.Query["f"], httpContext.Request.Query["n"]);
                        break;
                    case "GENERATETHUMB":
                        await _roxyFilemanService.CreateImageThumbnailAsync(httpContext.Request.Query["f"]);
                        break;
                    case "UPLOAD":
                        await _roxyFilemanService.UploadFilesAsync(httpContext.Request.Form["d"]);
                        break;
                    default:
                        await httpContext.Response.WriteAsync(_roxyFilemanService.GetErrorResponse("This action is not implemented."));
                        break;
                }
            }
            catch (Exception ex)
            {
                if (action == "UPLOAD" && !_roxyFilemanService.IsAjaxRequest())
                    await httpContext.Response.WriteAsync($"<script>parent.fileUploaded({_roxyFilemanService.GetErrorResponse(await _roxyFilemanService.GetLanguageResourceAsync("E_UploadNoFiles"))});</script>");
                else
                    await httpContext.Response.WriteAsync(_roxyFilemanService.GetErrorResponse(ex.Message));
            }
        }

        #endregion
    }
}