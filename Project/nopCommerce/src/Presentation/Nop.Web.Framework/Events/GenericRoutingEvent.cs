using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Seo;
using Nop.Core.Events;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs when a generic route is processed and before default transformation
/// </summary>
public partial class GenericRoutingEvent : IStopProcessingEvent
{
    #region Ctor

    public GenericRoutingEvent(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord)
    {
        HttpContext = httpContext;
        RouteValues = values;
        UrlRecord = urlRecord;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets HTTP context
    /// </summary>
    public HttpContext HttpContext { get; protected set; }

    /// <summary>
    /// Gets route values associated with the current match
    /// </summary>
    public RouteValueDictionary RouteValues { get; protected set; }

    /// <summary>
    /// Gets record found by the URL slug
    /// </summary>
    public UrlRecord UrlRecord { get; protected set; }

    /// <summary>
    /// Gets or sets a value whether processing of event publishing should be stopped
    /// </summary>
    public bool StopProcessing { get; set; }

    #endregion
}