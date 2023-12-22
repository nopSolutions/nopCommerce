using Microsoft.AspNetCore.Routing;

//code from Telerik MVC Extensions
namespace Nop.Web.Framework.Menu;

/// <summary>
/// Sitemap node
/// </summary>
public partial class SiteMapNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SiteMapNode"/> class.
    /// </summary>
    public SiteMapNode()
    {
        RouteValues = new RouteValueDictionary();
        ChildNodes = new List<SiteMapNode>();
    }

    /// <summary>
    /// Gets or sets the system name.
    /// </summary>
    public string SystemName { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the name of the controller.
    /// </summary>
    public string ControllerName { get; set; }

    /// <summary>
    /// Gets or sets the name of the action.
    /// </summary>
    public string ActionName { get; set; }

    /// <summary>
    /// Gets or sets the route values.
    /// </summary>
    public RouteValueDictionary RouteValues { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the child nodes.
    /// </summary>
    public IList<SiteMapNode> ChildNodes { get; set; }

    /// <summary>
    /// Gets or sets the icon class (Font Awesome: http://fontawesome.io/)
    /// </summary>
    public string IconClass { get; set; }

    /// <summary>
    /// Gets or sets the item is visible
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to open url in new tab (window) or not
    /// </summary>
    public bool OpenUrlInNewTab { get; set; }
}