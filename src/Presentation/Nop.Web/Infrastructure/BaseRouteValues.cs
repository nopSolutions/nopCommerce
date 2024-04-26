namespace Nop.Web.Infrastructure;

/// <summary>
/// Base custom RouteValues object
/// </summary>
public partial record BaseRouteValues : IRouteValues
{
    /// <summary>
    /// The page number
    /// </summary>
    public int PageNumber { get; set; }
}