using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents an artificial intelligence product full description generator model
/// </summary>
public partial record ArtificialIntelligenceFullDescriptionModel : BaseNopModel
{
    public ArtificialIntelligenceFullDescriptionModel()
    {
        AvailableLanguages = new List<SelectListItem>();
    }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.ProductName")]
    public string ProductName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.Keywords")]
    public string Keywords { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.ToneOfVoice")]
    public int ToneOfVoiceId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.CustomToneOfVoice")]
    public string CustomToneOfVoice { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.Instructions")]
    public string Instructions { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.GeneratedDescription")]
    public string GeneratedDescription { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.Language")]
    public int TargetLanguageId { get; set; }
    public IList<SelectListItem> AvailableLanguages { get; set; }

    public int LanguageId { get; set; }

    public bool SaveButtonClicked { get; set; }
}