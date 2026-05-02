using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents view component to use artificial intelligence service to generate product's full description
/// </summary>
public partial class FullDescriptionGeneratorViewComponent : NopViewComponent
{
    #region Filds

    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;

    #endregion

    #region Ctor

    public FullDescriptionGeneratorViewComponent(ArtificialIntelligenceSettings artificialIntelligenceSettings)
    {
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync(FullDescriptionGeneratorModel model)
    {
        if (!_artificialIntelligenceSettings.Enabled)
            return Content(string.Empty);

        return await ViewAsync(model);
    }

    #endregion
}
