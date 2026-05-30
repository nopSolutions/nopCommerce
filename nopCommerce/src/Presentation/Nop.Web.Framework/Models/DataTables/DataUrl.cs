using Microsoft.AspNetCore.Routing;

namespace Nop.Web.Framework.Models.DataTables;

/// <summary>
/// Represents the data url
/// </summary>
public partial class DataUrl
{
    #region Ctor

    /// <summary>
    /// Initializes a new instance of the DataUrl class 
    /// </summary>
    /// <param name="actionName">Action name</param>
    /// <param name="controllerName">Controller name</param>
    /// <param name="routeValues">Route values</param>
    public DataUrl(string actionName, string controllerName, RouteValueDictionary routeValues)
    {
        ActionName = actionName;
        ControllerName = controllerName;
        RouteValues = routeValues;
    }

    /// <summary>
    /// Initializes a new instance of the DataUrl class 
    /// </summary>
    /// <param name="url">URL</param>
    public DataUrl(string url)
    {
        Url = url;
    }

    /// <summary>
    /// Initializes a new instance of the DataUrl class 
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="dataId">Name of the column whose value is to be used as identifier in URL</param>
    public DataUrl(string url, string dataId)
    {
        Url = url;
        DataId = dataId;
    }

    /// <summary>
    /// Initializes a new instance of the DataUrl class 
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="trimEnd">Parameter indicating that you need to delete all occurrences of the character "/" at the end of the line</param>
    public DataUrl(string url, bool trimEnd)
    {
        Url = url;
        TrimEnd = trimEnd;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the name of the action.
    /// </summary>
    public string ActionName { get; set; }

    /// <summary>
    /// Gets or sets the name of the controller.
    /// </summary>
    public string ControllerName { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the route values.
    /// </summary>
    public RouteValueDictionary RouteValues { get; set; }

    /// <summary>
    /// Gets or sets data Id
    /// </summary>
    public string DataId { get; set; }

    /// <summary>
    /// Gets or sets parameter indicating that you need to delete all occurrences of the character "/" at the end of the line
    /// </summary>
    public bool TrimEnd { get; set; }

    #endregion
}