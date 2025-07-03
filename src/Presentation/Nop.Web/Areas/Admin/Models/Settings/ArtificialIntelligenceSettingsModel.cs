using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a artificial intelligence settings model
/// </summary>
public partial record ArtificialIntelligenceSettingsModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ArtificialIntelligence.Enable")]
    public bool Enabled { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ArtificialIntelligence.ProviderType")]
    public int ProviderTypeId { get; set; }
    public IList<SelectListItem> AvailableProviderType { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ArtificialIntelligence.GeminiApiKey")]
    [DataType(DataType.Password)]
    public string GeminiApiKey { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ArtificialIntelligence.ChatGptApiKey")]
    [DataType(DataType.Password)]
    public string ChatGptApiKey { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ArtificialIntelligence.DeepSeekApiKey")]
    [DataType(DataType.Password)]
    public string DeepSeekApiKey { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ArtificialIntelligence.ProductDescriptionQuery")]
    public string ProductDescriptionQuery { get; set; }

    #endregion
}
