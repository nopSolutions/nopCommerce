using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Media.ElFinder;
using Nop.Services.Security;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

/// <summary>
/// Controller for elFinder file manager
/// </summary>
public partial class ElFinderController : BaseAdminController
{
    #region Fields

    protected readonly IElFinderService _elFinderService;
    protected readonly IPermissionService _permissionService;    

    #endregion

    #region Ctor

    public ElFinderController(IElFinderService elFinderService, 
        IPermissionService permissionService)
    {
        _elFinderService = elFinderService;
        _permissionService = permissionService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle elFinder connector request
    /// </summary>
    [HttpPost]
    [HttpGet]
    [IgnoreAntiforgeryToken]
    [CheckPermission(StandardPermission.System.HTML_EDITOR_MANAGE_PICTURES)]
    public virtual async Task<IActionResult> Connector()
    {
        try
        {
            // This code will only work with the System.Text.Json serializer
            //var connector = await _elFinderService.GetConnectorAsync(Request);
            //return await connector.ProcessAsync(Request);

            var connector = await _elFinderService.GetConnectorAsync(Request);
            var response = await connector.ProcessAsync(Request);

            // If the library has already returned a serialized ContentResult, just return it
            if (response is ContentResult)
                return response;

            // If it's an ObjectResult (or JsonResult), we manually serialize it via System.Text.Json
            if (response is ObjectResult objResult)
            {
                var value = objResult.Value;
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    PropertyNameCaseInsensitive = true,
                };

                var json = JsonSerializer.Serialize(value, options);

                return new ContentResult
                {
                    Content = json,
                    ContentType = "application/json; charset=utf-8",
                    StatusCode = objResult.StatusCode ?? 200
                };
            }

            if (response is JsonResult jsonResult)
            {
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(jsonResult.Value, options);
                return Content(json, "application/json; charset=utf-8");
            }

            // For other types (FileResult, RedirectResult, etc.) - return as is
            return response;

        }
        catch (Exception ex)
        {
            // Return error in JSON format that elFinder expects
            return Json(new { error = new[] { ex.Message } });
        }
    }

    public async Task<IActionResult> Thumb()
    {
        try
        {
            var connector = await _elFinderService.GetConnectorAsync(Request);

            var path = Request.Path.Value;
            var lastSegment = path[(path.LastIndexOf('/') + 1)..];

            return await connector.GetThumbnailAsync(HttpContext.Request, HttpContext.Response, lastSegment);
        }
        catch (Exception ex)
        {
            return Json(new { error = "Unable to process request: " + ex.Message });
        }
    }

    #endregion
}
