using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents view component to use artificial intelligence service to generate meta tags
/// </summary>
public partial class MetaTagsGeneratorViewComponent : NopViewComponent
{
    #region Filds

    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;

    #endregion

    #region Ctor

    public MetaTagsGeneratorViewComponent(ArtificialIntelligenceSettings artificialIntelligenceSettings)
    {
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
    }

    #endregion

    #region Methods

    public IViewComponentResult Invoke(MetaTagsGeneratorModel model)
    {
        if (!_artificialIntelligenceSettings.Enabled)
            return Content(string.Empty);

        if (!_artificialIntelligenceSettings.AllowMetaTitleGeneration &&
            !_artificialIntelligenceSettings.AllowMetaKeywordsGeneration &&
            !_artificialIntelligenceSettings.AllowMetaDescriptionGeneration)
        {
            return Content(string.Empty);
        }

        return View(model);
    }

    #endregion
}