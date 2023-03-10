using Nop.Web.Infrastructure;

namespace Nop.Web.Models.Common;

/// <summary>
/// record that has a slug and page for route values. Used for Topic (posts) and 
/// Forum (topics) pagination
/// </summary>
public partial record SlugRouteValues : BaseRouteValues
{
    public int Id { get; set; }

    public string Slug { get; set; }
}