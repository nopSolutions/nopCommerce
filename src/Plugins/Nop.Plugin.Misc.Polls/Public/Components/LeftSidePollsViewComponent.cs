using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Polls.Public.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.Polls.Public.Components;

/// <summary>
/// Represents a view component for displaying polls in the left sidebar
/// </summary>
public class LeftSidePollsViewComponent : NopViewComponent
{
    #region Fields

    private readonly PollModelFactory _pollModelFactory;
    private readonly PollSettings _pollSettings;

    #endregion

    #region Ctor

    public LeftSidePollsViewComponent(PollModelFactory pollModelFactory, PollSettings pollSettings)
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

        var model = await _pollModelFactory.PrepareLeftSidePollModelAsync();
        if (model is null)
            return Content(string.Empty);

        return View("~/Plugins/Misc.Polls/Public/Views/PollBlock.cshtml", model);
    }

    #endregion
}