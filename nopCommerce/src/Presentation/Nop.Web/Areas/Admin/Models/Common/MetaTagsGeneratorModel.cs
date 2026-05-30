using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents a meta tags generator model
/// </summary>
public partial record MetaTagsGeneratorModel : BaseNopModel
{
    #region Ctor

    public MetaTagsGeneratorModel()
    {
    }

    public MetaTagsGeneratorModel(IMetaTagsSupportedModel model, string entityTypeName, int languagePosition = 0, int languageId = 0)
    {
        LanguageId = languageId;
        EntityType = entityTypeName;

        if (model is BaseNopEntityModel entityModel)
            EntityId = entityModel.Id;

        var prefix = languageId > 0 ? $"Locales_{languagePosition}__" : string.Empty;

        MetaTitleElementId = $"{prefix}{nameof(model.MetaTitle)}";
        MetaKeywordsElementId = $"{prefix}{nameof(model.MetaKeywords)}";
        MetaDescriptionElementId = $"{prefix}{nameof(model.MetaDescription)}";
    }

    #endregion

    #region Properties

    public string TitleFieldId { get; set; }
    public string TextFieldId { get; set; }
    public string EntityType { get; set; }
    public int EntityId { get; set; }
    public int LanguageId { get; set; }

    public string MetaKeywordsElementId { get; set; }
    public string MetaDescriptionElementId { get; set; }
    public string MetaTitleElementId { get; set; }

    #endregion
}