using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.WebhookSettings;

namespace Nop.Web.Areas.Admin.Controllers;

public class WebhookSettingController : BaseAdminController
{
    private readonly WebhookSettings _webhookSettings;

    public WebhookSettingController(WebhookSettings webhookSettings)
    {
        _webhookSettings = webhookSettings;
    }

    public IActionResult Index()
    {
        
        return View();
    }

}