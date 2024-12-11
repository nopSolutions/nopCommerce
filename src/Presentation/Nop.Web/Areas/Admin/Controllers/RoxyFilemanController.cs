using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Media;

namespace Nop.Web.Areas.Admin.Controllers;
//Controller for Roxy fileman (http://www.roxyfileman.com/) for TinyMCE editor
//the original file was \RoxyFileman-1.4.5-net\fileman\asp_net\main.ashx

//do not validate request token (XSRF)
public partial class RoxyFilemanController : BaseAdminController
{
    #region Fields

    protected readonly IPermissionService _permissionService;
    protected readonly IRoxyFilemanService _roxyFilemanService;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public RoxyFilemanController(IPermissionService permissionService, IRoxyFilemanService roxyFilemanService, IWebHelper webHelper)
    {
        _permissionService = permissionService;
        _roxyFilemanService = roxyFilemanService;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    protected virtual JsonResult JsonOk()
    {
        return Json(new { res = "ok" });
    }

    protected virtual JsonResult JsonError(string message)
    {
        return Json(new { res = "error", msg = message });
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create configuration file for RoxyFileman
    /// </summary>
    public virtual async Task CreateConfiguration()
    {
        var currentPathBase = Request.PathBase.ToString();
        await _roxyFilemanService.ConfigureAsync(currentPathBase);
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> DirectoriesList(string type)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            var directories = _roxyFilemanService.GetDirectoryList(type);
            return Json(directories);
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> FilesList(string d, string type)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            var directories = _roxyFilemanService.GetFiles(d, type);

            return Json(directories);
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    public virtual async Task<IActionResult> DownloadFile(string f)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            var (stream, name) = _roxyFilemanService.GetFileStream(f);

            if (!new FileExtensionContentTypeProvider().TryGetContentType(f, out var contentType))
                contentType = MimeTypes.ApplicationOctetStream;

            return File(stream, contentType, name);
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> CopyDirectory(string d, string n)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            _roxyFilemanService.CopyDirectory(d, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> CopyFile(string f, string n)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            _roxyFilemanService.CopyFile(f, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> CreateDirectory(string d, string n)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            _roxyFilemanService.CreateDirectory(d, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> DeleteDirectory(string d)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            _roxyFilemanService.DeleteDirectory(d);

            return JsonOk();
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> DeleteFile(string f)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            _roxyFilemanService.DeleteFile(f);

            return JsonOk();
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    public virtual async Task<IActionResult> DownloadDirectory(string d)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
            throw new Exception("You don't have required permission");

        var fileContents = _roxyFilemanService.DownloadDirectory(d);

        return File(fileContents, MimeTypes.ApplicationZip);
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> MoveDirectory(string d, string n)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            _roxyFilemanService.MoveDirectory(d, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> MoveFile(string f, string n)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            _roxyFilemanService.MoveFile(f, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> RenameDirectory(string d, string n)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            _roxyFilemanService.RenameDirectory(d, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> RenameFile(string f, string n)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            _roxyFilemanService.RenameFile(f, n);

            return JsonOk();
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    public virtual async Task<IActionResult> CreateImageThumbnail(string f)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(f, out var contentType))
                contentType = MimeTypes.ImageJpeg;

            var fileContents = _roxyFilemanService.CreateImageThumbnail(f, contentType);

            return File(fileContents, contentType);
        }
        catch (Exception ex)
        {
            return JsonError(ex.Message);
        }
    }

    [IgnoreAntiforgeryToken]
    [HttpPost]
    public virtual async Task<IActionResult> UploadFiles([FromForm] RoxyFilemanUploadModel uploadModel)
    {
        try
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES))
                throw new Exception("You don't have required permission");

            var form = await HttpContext.Request.ReadFormAsync();

            if (!form.Files.Any())
                throw new RoxyFilemanException("E_UploadNoFiles");

            await _roxyFilemanService.UploadFilesAsync(uploadModel.D, form.Files);

            return JsonOk();
        }
        catch (Exception ex)
        {
            if (!_webHelper.IsAjaxRequest(Request))
            {
                var roxyError = new { res = "error", msg = ex.Message };
                return Content($"<script>parent.fileUploaded({JsonConvert.SerializeObject(roxyError)});</script>");
            }

            return JsonError(ex.Message);
        }
    }

    #endregion

}