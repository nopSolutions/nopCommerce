using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.AIRecommendation.GoogleAI.Models;

public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugin.AIRecommendation.GoogleAI.Enabled")]
    public bool Enabled { get; set; }

    [NopResourceDisplayName("Plugin.AIRecommendation.GoogleAI.ProjectId")]
    public string ProjectId { get; set; }

    [NopResourceDisplayName("Plugin.AIRecommendation.GoogleAI.LocationId")]
    public string LocationId { get; set; }

    [NopResourceDisplayName("Plugin.AIRecommendation.GoogleAI.CatalogId")]
    public string CatalogId { get; set; }

    [NopResourceDisplayName("Plugin.AIRecommendation.GoogleAI.BranchId")]
    public string BranchId { get; set; }

    [NopResourceDisplayName("Plugin.AIRecommendation.GoogleAI.SyncAllowed")]
    public bool SyncAllowed { get; set; }

    [NopResourceDisplayName("Plugin.AIRecommendation.GoogleAI.LogRequests")]
    public bool LogRequests { get; set; }

    [NopResourceDisplayName("Plugin.AIRecommendation.GoogleAI.SearchAllowed")]
    public bool SearchAllowed { get; set; }
}
