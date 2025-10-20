using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Brevo.Models;

/// <summary>
/// Represents a model for mapping newsletter subscriptions
/// </summary>
public record NewsLetterSubscriptionMapModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the type identifier
    /// </summary>
    public int TypeId { get; set; }

    /// <summary>
    /// Gets or sets the name of the list
    /// </summary>
    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.List")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the list identifier
    /// </summary>
    public int ListId { get; set; }
}
