using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Polls.Public.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.Polls.Public.Components;

/// <summary>
/// Represents a view component for displaying polls on the homepage
/// </summary>
public class HomepagePollsViewComponent : NopViewComponent
{
    #region Fields

    private readonly PollModelFactory _pollModelFactory;
    private readonly PollSettings _pollSettings;

    #endregion

    #region Ctor

    public HomepagePollsViewComponent(PollModelFactory pollModelFactory, PollSettings pollSettings)
    {
        _pollModelFactory = pollModelFactory;
        _pollSettings = pollSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke the view component
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!_pollSettings.Enabled)
            return Content(string.Empty);

        var model = await _pollModelFactory.PrepareHomepagePollModelsAsync();
        if (!model.Any())
            return Content(string.Empty);

        return View("~/Plugins/Misc.Polls/Public/Views/HomepagePolls.cshtml", model);
    }

    #endregion
}