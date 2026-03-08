using Nop.Web.Infrastructure;

namespace Nop.Plugin.Misc.Forums.Public.Models;

/// <summary>
/// record that has a slug and page for route values. Used for Topic (posts) and 
/// Forum (topics) pagination
/// </summary>
public record SlugRouteValues : BaseRouteValues
{
    #region Properties

    public int Id { get; set; }
    public string Slug { get; set; }

    #endregion
}