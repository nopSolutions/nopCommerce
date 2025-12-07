using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Services;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ServicesProvidedController : BaseAdminController
{
    private const string ServicesFileName = "services-provided.json";

    private readonly INopFileProvider _fileProvider;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IPermissionService _permissionService;

    public ServicesProvidedController(
        INopFileProvider fileProvider,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
        _fileProvider = fileProvider;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
    }

    protected virtual string GetServicesFilePath()
    {
        return _fileProvider.MapPath($"~/App_Data/{ServicesFileName}");
    }

    protected virtual string GetDefaultPayload()
    {
        var sample = new
        {
            services = new object[]
            {
                new
                {
                    name = "Neuro Music Therapy",
                    treatmentCode = "HPCSA 2001",
                    description = "Intensive sensory engagement designed for neuro-rehab clients.",
                    consumables = new[]
                    {
                        new { name = "Instrument sterilisation", code = "INSTR-STER", quantity = "Per session" },
                        new { name = "Adaptive headset", code = "HPCSA A12", quantity = "On demand" }
                    },
                    subtypes = new object[]
                    {
                        new
                        {
                            name = "15-30 minutes Music Therapy",
                            treatmentCode = "HPCSA M-30",
                            consumables = new[]
                            {
                                new { name = "Disposable picks", code = "CONS-45", quantity = "2" },
                                new { name = "Keyboard skin", code = "CONS-91", quantity = "1" }
                            }
                        },
                        new
                        {
                            name = "45-60 minutes Music Therapy",
                            treatmentCode = "HPCSA M-60",
                            consumables = new[]
                            {
                                new { name = "Instrument wrap", code = "CONS-47", quantity = "1" }
                            }
                        }
                    }
                },
                new
                {
                    name = "Speech & Language Therapy",
                    treatmentCode = "HPCSA 2103",
                    description = "Responsive speech sessions that track consumables for billing.",
                    consumables = new[]
                    {
                        new { name = "TheraBands", code = "HPCSA B10", quantity = "Per plan" },
                        new { name = "Mirror shield", code = "HPCSA B18", quantity = "Reusable" }
                    },
                    subtypes = Array.Empty<object>()
                }
            }
        };

        return JsonSerializer.Serialize(sample, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    [HttpGet]
    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_SETTINGS))
            return AccessDeniedView();

        var filePath = GetServicesFilePath();
        var body = string.Empty;

        if (System.IO.File.Exists(filePath))
        {
            body = await System.IO.File.ReadAllTextAsync(filePath);
        }
        else
        {
            body = GetDefaultPayload();
        }

        var model = new ServicesProvidedModel
        {
            ServicesJson = body,
            SampleJson = GetDefaultPayload()
        };

        return View(model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    public async Task<IActionResult> ConfigurePost(ServicesProvidedModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_SETTINGS))
            return AccessDeniedView();

        if (!string.IsNullOrWhiteSpace(model.ServicesJson))
        {
            try
            {
                _ = JToken.Parse(model.ServicesJson);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(model.ServicesJson),
                    string.Format("Invalid services payload: {0}", ex.Message));
            }
        }

        if (!ModelState.IsValid)
            return View("Configure", model);

        var path = GetServicesFilePath();
        _fileProvider.CreateFile(path);
        await _fileProvider.WriteAllTextAsync(path, model.ServicesJson ?? string.Empty, Encoding.UTF8);

        _notificationService.SuccessNotification(
            await _localizationService.GetResourceAsync("Admin.ServicesProvided.Updated", 0, true, "Services provided updated"));

        return RedirectToAction("Configure");
    }
}
