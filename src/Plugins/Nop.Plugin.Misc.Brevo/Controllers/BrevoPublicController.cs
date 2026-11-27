using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Caching;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Brevo.Controllers;

[AutoValidateAntiforgeryToken]
public class BrevoPublicController : BasePluginController
{
    #region Fields

    protected readonly ILogger _logger;
    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public BrevoPublicController(ILogger logger,
        IStaticCacheManager staticCacheManager)
    {
        _logger = logger;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> ImportContacts(BaseNopModel model, IFormCollection form)
    {
        try
        {
            //logging info
            var logInfo = string.Format("Brevo synchronization: New emails {1},{0} Existing emails {2},{0} Invalid emails {3},{0} Duplicates emails {4}{0}",
                Environment.NewLine, form["new_emails"], form["emails_exists"], form["invalid_email"], form["duplicates_email"]);
            await _logger.InformationAsync(logInfo);

            //display info on configuration page in case of the manually synchronization
            await _staticCacheManager.SetAsync(_staticCacheManager.PrepareKeyForDefaultCache(BrevoDefaults.SyncKeyCache), logInfo);
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync(ex.Message, ex);
            await _staticCacheManager.SetAsync(_staticCacheManager.PrepareKeyForDefaultCache(BrevoDefaults.SyncKeyCache), ex.Message);
        }

        return Ok();
    }

    #endregion
}