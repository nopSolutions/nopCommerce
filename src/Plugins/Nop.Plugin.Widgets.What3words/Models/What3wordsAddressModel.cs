using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.What3words.Models;

/// <summary>
/// Represents what3words address model
/// </summary>
public record What3wordsAddressModel : BaseNopModel
{
    public string ApiKey { get; set; }

    public string Prefix { get; set; }
}